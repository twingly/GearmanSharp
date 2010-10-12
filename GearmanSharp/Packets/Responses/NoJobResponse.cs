using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman.Packets
{
    public class NoJobResponse : ResponsePacket
    {
        public NoJobResponse(byte[] packetData)
            : base(PacketType.NO_JOB, packetData)
        {
        }
    }
}