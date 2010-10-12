using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

namespace Twingly.Gearman.Packets
{
    public abstract class RequestPacket : Packet
    {
        public static readonly byte[] Magic = new byte[4] { 0, (byte)'R', (byte)'E', (byte)'Q' };

        public override byte[] GetMagic()
        {
            return Magic;
        }

        protected RequestPacket(PacketType packetType) : base(packetType)
        {
        }
    }
}