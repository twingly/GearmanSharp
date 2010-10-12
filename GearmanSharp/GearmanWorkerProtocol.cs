using System.Linq;
using System.Linq.Expressions;
using Twingly.Gearman.Exceptions;
using Twingly.Gearman.Packets;

namespace Twingly.Gearman
{
    public class JobAssignment
    {
        public string JobHandle;
        public string FunctionName;
        public byte[] FunctionArgument;
    }

    public class GearmanWorkerProtocol
    {
        private readonly IGearmanConnection _connection;

        public GearmanWorkerProtocol(IGearmanConnection connection)
        {
            _connection = connection;
        }

        public void SetClientId(string clientId)
        {
            _connection.SendPacket(new SetClientIdRequest(clientId));
        }

        public void CanDo(string functionName)
        {
            _connection.SendPacket(new CanDoRequest(functionName));
        }

        public JobAssignment GrabJob()
        {
            _connection.SendPacket(new GrabJobRequest());

            IResponsePacket response;
            do
            {
                response = _connection.GetNextPacket(); // Throw away all NOOPs.
            } while (response.Type == PacketType.NOOP);

            if (response.Type == PacketType.ERROR)
            {
                var errorPacket = (ErrorResponse)response;
                throw new GearmanServerException(errorPacket.ErrorCode, errorPacket.ErrorText);
            }

            JobAssignment job;
            if (response.Type == PacketType.JOB_ASSIGN)
            {
                var jobAssign = (IJobAssignResponse)response;
                job = new JobAssignment
                      {
                          JobHandle = jobAssign.JobHandle,
                          FunctionName = jobAssign.FunctionName,
                          FunctionArgument = jobAssign.FunctionArgument
                      };
            }
            else if (response.Type == PacketType.NO_JOB)
            {
                job = null;
            }
            else
            {
                throw new GearmanApiException("Got unknown packet from server");
            }

            return job;
        }

        public void WorkComplete(string jobHandle)
        {
            WorkComplete(jobHandle, null);
        }

        public void WorkComplete(string jobHandle, byte[] result)
        {
            _connection.SendPacket(new WorkCompleteRequest(jobHandle, result ?? new byte[0]));
        }

        public void WorkFail(string jobHandle)
        {
            _connection.SendPacket(new WorkFailRequest(jobHandle));
        }
    }
}