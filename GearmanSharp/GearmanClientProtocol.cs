using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Twingly.Gearman.Exceptions;
using Twingly.Gearman.Packets;

namespace Twingly.Gearman
{
    public abstract class GearmanProtocol
    {
        public IGearmanConnection Connection { get; protected set; }

        protected GearmanProtocol(IGearmanConnection connection)
        {
            Connection = connection;
        }

        public static GearmanServerException UnpackErrorReponse(IResponsePacket response)
        {
            var args = Util.SplitArray(response.GetData());
            throw new GearmanServerException(Encoding.UTF8.GetString(args[0]), Encoding.UTF8.GetString(args[1]));
        }

        public static RequestPacket PackRequest(PacketType packetType)
        {
            return new RequestPacket(packetType);
        }

        public static RequestPacket PackRequest(PacketType packetType, string arg1)
        {
            if (arg1 == null)
                throw new ArgumentNullException("arg1");

            return new RequestPacket(packetType, Encoding.UTF8.GetBytes(arg1));
        }

        public static RequestPacket PackRequest(PacketType packetType, string arg1, byte[] arg2)
        {
            if (arg1 == null)
                throw new ArgumentNullException("arg1");

            if (arg2 == null)
                throw new ArgumentNullException("arg2");

            return new RequestPacket(packetType, JoinByteArraysForData(Encoding.UTF8.GetBytes(arg1), arg2));
        }

        public static RequestPacket PackRequest(PacketType packetType, string arg1, string arg2)
        {
            if (arg1 == null)
                throw new ArgumentNullException("arg1");

            if (arg2 == null)
                throw new ArgumentNullException("arg2");

            return new RequestPacket(packetType, JoinByteArraysForData(Encoding.UTF8.GetBytes(arg1), Encoding.UTF8.GetBytes(arg1)));
        }

        public static RequestPacket PackRequest(PacketType packetType, string arg1, string arg2, string arg3)
        {
            if (arg1 == null)
                throw new ArgumentNullException("arg1");

            if (arg2 == null)
                throw new ArgumentNullException("arg2");

            if (arg3 == null)
                throw new ArgumentNullException("arg3");

            return new RequestPacket(packetType,
                JoinByteArraysForData(Encoding.UTF8.GetBytes(arg1), Encoding.UTF8.GetBytes(arg2), Encoding.UTF8.GetBytes(arg3)));
        }

        public static RequestPacket PackRequest(PacketType packetType, string arg1, string arg2, byte[] arg3)
        {
            if (arg1 == null)
                throw new ArgumentNullException("arg1");

            if (arg2 == null)
                throw new ArgumentNullException("arg2");

            if (arg3 == null)
                throw new ArgumentNullException("arg3");

            return new RequestPacket(packetType,
                JoinByteArraysForData(Encoding.UTF8.GetBytes(arg1), Encoding.UTF8.GetBytes(arg2), arg3));
        }

        public static string UnpackJobCreatedResponse(IResponsePacket response)
        {
            return Encoding.UTF8.GetString(response.GetData());
        }

        public static GearmanJobInfo UnpackJobAssignResponse(IResponsePacket response)
        {
            var args = Util.SplitArray(response.GetData());
            return new GearmanJobInfo
                  {
                      JobHandle = Encoding.UTF8.GetString(args[0]),
                      FunctionName = Encoding.UTF8.GetString(args[1]),
                      FunctionArgument = args[2]
                  };
        }
        
        public static GearmanJobStatus UnpackStatusResponse(IResponsePacket response)
        {
            var args = Util.SplitArray(response.GetData());
            return new GearmanJobStatus(
                Encoding.UTF8.GetString(args[0]),
                uint.Parse(Encoding.UTF8.GetString(args[1])) == 0 ? false : true,
                uint.Parse(Encoding.UTF8.GetString(args[2])) == 0 ? false : true,
                uint.Parse(Encoding.UTF8.GetString(args[3])),
                uint.Parse(Encoding.UTF8.GetString(args[4])));
        }

        public static GearmanJobData UnpackWorkDataResponse(IResponsePacket response)
        {
            var args = Util.SplitArray(response.GetData());
            return new GearmanJobData(Encoding.UTF8.GetString(args[0]), args[1]);
        }

        public static GearmanJobData UnpackWorkCompleteResponse(IResponsePacket response)
        {
            return UnpackWorkDataResponse(response);
        }

        /// <summary>
        /// Concatenates a number of byte arrays with \0 between them.
        /// </summary>
        public static byte[] JoinByteArraysForData(params byte[][] data)
        {
            const byte splitByte = 0;

            int len = (data.Length == 0 ? 0 : data.Length - 1);
            foreach (var arr in data)
            {
                len += arr.Length;
            }

            var result = new byte[len];
            var offset = 0;
            bool first = true;
            foreach (var arr in data)
            {
                // Add \0 before all values, except for the first. (i.e. append it for all but the last)
                if (first)
                    first = false;
                else
                    result[offset++] = splitByte;
                Array.Copy(arr, 0, result, offset, arr.Length);
                offset += arr.Length;
            }

            return result;
        }
    }

    public class GearmanJobData
    {
        public string JobHandle { get; protected set; }
        public byte[] Data { get; protected set; }

        public GearmanJobData(string jobHandle, byte[] data)
        {
            JobHandle = jobHandle;
            Data = data;
        }
    }

    public class GearmanClientProtocol : GearmanProtocol
    {
        public GearmanClientProtocol(IGearmanConnection connection)
            : base(connection)
        {
        }

        public string SubmitBackgroundJob(string functionName, byte[] functionArgument, string uniqueId, GearmanJobPriority priority)
        {
            return SubmitJob(functionName, functionArgument, true, uniqueId, priority);
        }

        private string SubmitJob(string functionName, byte[] functionArgument, bool background, string uniqueId, GearmanJobPriority priority)
        {
            if (functionName == null)
                throw new ArgumentNullException("functionName");

            Connection.SendPacket(PackRequest(
                GetSubmitJobType(priority, background),
                functionName,
                uniqueId ?? "",
                functionArgument ?? new byte[0]));
            var response = Connection.GetNextPacket();

            switch (response.Type)
            {
                case PacketType.JOB_CREATED:
                    return UnpackJobCreatedResponse(response);
                case PacketType.ERROR:
                    throw UnpackErrorReponse(response);
                default:
                    throw new GearmanApiException("Got unknown packet from server");
            }
        }

        private static PacketType GetSubmitJobType(GearmanJobPriority priority, bool background)
        {
            switch (priority)
            {
                case GearmanJobPriority.High:
                    return background ? PacketType.SUBMIT_JOB_HIGH_BG : PacketType.SUBMIT_JOB_HIGH;
                case GearmanJobPriority.Normal:
                    return background ? PacketType.SUBMIT_JOB_BG : PacketType.SUBMIT_JOB;
                case GearmanJobPriority.Low:
                    return background ? PacketType.SUBMIT_JOB_LOW_BG : PacketType.SUBMIT_JOB_LOW;
                default:
                    throw new GearmanApiException("Unknown priority and background combination for SubmitJobRequest");
            }
        }

        public byte[] SubmitJob(string functionName, byte[] functionArgument, string uniqueId, GearmanJobPriority priority)
        {
            var jobHandle = SubmitJob(functionName, functionArgument, false, uniqueId, priority);

            var result = new List<byte>();
            var workDone = false;
            while (!workDone)
            {
                var response = Connection.GetNextPacket();

                // TODO: Check that we received a response for/with the same job handle?

                switch (response.Type)
                {
                    case PacketType.WORK_FAIL:
                        // Do what? Return null?  (should not throw)
                        return null;
                    case PacketType.WORK_COMPLETE:
                        var workComplete = UnpackWorkCompleteResponse(response);
                        result.AddRange(workComplete.Data);
                        workDone = true;
                        break;
                    case PacketType.WORK_DATA:
                        var workData = UnpackWorkDataResponse(response);
                        result.AddRange(workData.Data);
                        break;
                    case PacketType.WORK_WARNING:
                    case PacketType.WORK_STATUS:
                    case PacketType.WORK_EXCEPTION:
                        // TODO: Do what?
                        break;
                    case PacketType.ERROR:
                        throw UnpackErrorReponse(response);
                    default:
                        throw new GearmanApiException("Got unknown packet from server");
                }   
            }

            return result.ToArray();
        }

        public GearmanJobStatus GetStatus(string jobHandle)
        {
            Connection.SendPacket(new RequestPacket(PacketType.GET_STATUS, Encoding.UTF8.GetBytes(jobHandle)));
            var response = Connection.GetNextPacket();

            switch (response.Type)
            {
                case PacketType.STATUS_RES:
                    return UnpackStatusResponse(response);
                case PacketType.ERROR:
                    throw UnpackErrorReponse(response);
                default:
                    throw new GearmanApiException("Got unknown packet from server");
            }
        }
    }
}