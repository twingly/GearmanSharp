using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman.Packets
{
    public abstract class JobHandleDataResponse : ResponsePacket
    {
        public string JobHandle { get; protected set; }
        public byte[] Data { get; protected set; }

        protected JobHandleDataResponse(PacketType packetType, byte[] packetData)
            : base(packetType, packetData)
        {
            string jobHandle;
            var nextOffset = ParseString(packetData, 0, out jobHandle);
            JobHandle = jobHandle;

            Data = packetData.Slice(nextOffset, packetData.Length);
        }
    }
}