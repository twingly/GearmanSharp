using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Twingly.Gearman.Exceptions;
using Twingly.Gearman.Packets;

namespace Twingly.Gearman
{
    public class GearmanClientProtocol
    {
        private readonly IGearmanConnection _connection;

        public GearmanClientProtocol(IGearmanConnection connection)
        {
            _connection = connection;
        }

        public string SubmitBackgroundJob(string functionName, byte[] functionArgument, string uniqueId, GearmanJobPriority priority)
        {
            return SubmitJob(functionName, functionArgument, true, uniqueId, priority);
        }

        private string SubmitJob(string functionName, byte[] functionArgument, bool background, string uniqueId, GearmanJobPriority priority)
        {
            _connection.SendPacket(new SubmitJobRequest(functionName, functionArgument, background, uniqueId, priority));
            var response = _connection.GetNextPacket();

            switch (response.Type)
            {
                case PacketType.JOB_CREATED:
                    var jobCreatedResponse = (JobCreatedResponse) response;
                    return jobCreatedResponse.JobHandle;
                case PacketType.ERROR:
                    var error = (ErrorResponse)response;
                    throw new GearmanServerException(error.ErrorCode, error.ErrorText);
                default:
                    throw new GearmanApiException("Got unknown packet from server");
            }
        }

        public byte[] SubmitJob(string functionName, byte[] functionArgument, string uniqueId, GearmanJobPriority priority)
        {
            var jobHandle = SubmitJob(functionName, functionArgument, false, uniqueId, priority);

            var result = new List<byte>();
            var workDone = false;
            while (!workDone)
            {
                var response = _connection.GetNextPacket();

                // TODO: Check that we received a response for/with the same job handle.

                switch (response.Type)
                {
                    case PacketType.WORK_FAIL:
                        // Do what? Throw exception? Return null?
                        return null;
                    case PacketType.WORK_COMPLETE:
                        var workCompleteResponse = (WorkCompleteResponse) response;
                        result.AddRange(workCompleteResponse.Data);
                        workDone = true;
                        break;
                    case PacketType.WORK_DATA:
                        var workDataResponse = (WorkDataResponse) response;
                        result.AddRange(workDataResponse.Data);
                        break;
                    case PacketType.WORK_WARNING:
                        // TODO: Do what?
                        break;
                    case PacketType.WORK_STATUS:
                    case PacketType.WORK_EXCEPTION:
                        // TODO: Do what?
                        break;
                    case PacketType.ERROR:
                        var error = (ErrorResponse)response;
                        throw new GearmanServerException(error.ErrorCode, error.ErrorText);
                    default:
                        throw new GearmanApiException("Got unknown packet from server");
                }   
            }

            return result.ToArray();
        }
    }
}