using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman.Packets
{
    public class NoopResponse : ResponsePacket
    {
        public NoopResponse(byte[] packetData)
            : base(PacketType.NOOP, packetData)
        {
        }
    }
}