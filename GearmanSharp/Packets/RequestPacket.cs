using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;

namespace Twingly.Gearman.Packets
{
    public class RequestPacket : Packet
    {
        public static readonly byte[] Magic = new byte[4] { 0, (byte)'R', (byte)'E', (byte)'Q' };

        public RequestPacket(PacketType packetType)
            : this(packetType, new byte[0])
        {
        }

        public RequestPacket(PacketType packetType, byte[] packetData)
            : base(packetType, packetData)
        {
        }

        public override byte[] GetMagic()
        {
            return Magic;
        }
    }
}