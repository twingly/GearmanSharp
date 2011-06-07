using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Twingly.Gearman.Exceptions;

namespace Twingly.Gearman.Packets
{
    public interface IResponsePacket
    {
        byte[] GetMagic();
        byte[] GetData();
        PacketType Type { get; }
        byte[] ToByteArray();
    }

    public class ResponsePacket : Packet, IResponsePacket
    {
        public static readonly byte[] Magic = new byte[4] { 0, (byte)'R', (byte)'E', (byte)'S' };

        public ResponsePacket(PacketType packetType, byte[] packetData)
            : base(packetType, packetData)
        {
        }

        public override byte[] GetMagic()
        {
            return Magic;
        }

        public static int ParseString(byte[] data, int startIndex, out string str)
        {
            int offset = startIndex;
            for (; offset < data.Length && data[offset] != 0; offset++) { }
            str = Encoding.UTF8.GetString(data.Slice(startIndex, offset));

            return offset + 1; // next position
        }
    }
}