using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Twingly.Gearman.Exceptions;
using Twingly.Gearman.Packets;

namespace Twingly.Gearman
{
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