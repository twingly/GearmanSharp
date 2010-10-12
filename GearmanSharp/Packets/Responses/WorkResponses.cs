using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman.Packets
{
    public class WorkFailResponse : ResponsePacket
    {
        public string JobHandle { get; protected set; }

        public WorkFailResponse(byte[] packetData)
            : base(PacketType.WORK_FAIL, packetData)
        {
            string jobHandle;
            ParseString(packetData, 0, out jobHandle);
            JobHandle = jobHandle;
        }
    }

    public class WorkCompleteResponse : JobHandleDataResponse
    {
        public WorkCompleteResponse(byte[] packetData) :
            base(PacketType.WORK_COMPLETE, packetData)
        {
        }
    }

    public class WorkDataResponse : JobHandleDataResponse
    {
        public WorkDataResponse(byte[] packetData)
            : base(PacketType.WORK_DATA, packetData)
        {
        }
    }

    public class WorkWarningResponse : JobHandleDataResponse
    {
        public WorkWarningResponse(byte[] packetData)
            : base(PacketType.WORK_WARNING, packetData)
        {
        }
    }

    public class WorkExceptionResponse : JobHandleDataResponse
    {
        public WorkExceptionResponse(byte[] packetData)
            : base(PacketType.WORK_EXCEPTION, packetData)
        {
        }
    }

    public class WorkStatusResponse : JobHandleDataResponse
    {
        public WorkStatusResponse(byte[] packetData)
            : base(PacketType.WORK_EXCEPTION, packetData)
        {
        }
    }
}