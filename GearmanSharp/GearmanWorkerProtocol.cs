using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Twingly.Gearman.Exceptions;
using Twingly.Gearman.Packets;

namespace Twingly.Gearman
{
    public class GearmanWorkerProtocol : GearmanProtocol
    {
        public GearmanWorkerProtocol(IGearmanConnection connection)
            : base(connection)
        {
        }

        public void SetClientId(string clientId)
        {
            Connection.SendPacket(PackRequest(PacketType.SET_CLIENT_ID, clientId));
        }

        public void CanDo(string functionName)
        {
            Connection.SendPacket(PackRequest(PacketType.CAN_DO, functionName));
        }

        public GearmanJobInfo GrabJob()
        {
            Connection.SendPacket(PackRequest(PacketType.GRAB_JOB));

            IResponsePacket response;
            do
            {
                response = Connection.GetNextPacket(); // Throw away all NOOPs.
            } while (response.Type == PacketType.NOOP);

            if (response.Type == PacketType.ERROR)
            {
                throw UnpackErrorReponse(response);
            }

            GearmanJobInfo job;
            if (response.Type == PacketType.JOB_ASSIGN)
            {
                job = UnpackJobAssignResponse(response);
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
            Connection.SendPacket(PackRequest(PacketType.WORK_COMPLETE, jobHandle, result ?? new byte[0]));
        }

        public void WorkFail(string jobHandle)
        {
            Connection.SendPacket(PackRequest(PacketType.WORK_FAIL, jobHandle));
        }

        public void WorkStatus(string jobHandle, uint numerator, uint denominator)
        {
            // The numerator and denominator should be sent as text, not binary.
            Connection.SendPacket(PackRequest(PacketType.WORK_STATUS, jobHandle, numerator.ToString(), denominator.ToString()));
        }
    }
}