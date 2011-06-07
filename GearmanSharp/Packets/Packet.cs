using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Twingly.Gearman.Packets
{
    public abstract class Packet
    {
        private readonly byte[] _packetData;

        public PacketType Type { get; protected set; }
        
        protected Packet(PacketType packetType, byte[] packetData)
        {
            Type = packetType;
            _packetData = packetData;
        }

        public abstract byte[] GetMagic();

        private byte[] GetHeader(int dataSize)
        {
            var header = new byte[12];
            Array.Copy(GetMagic(), 0, header, 0, 4);
            Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)Type)), 0, header, 4, 4);
            Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(dataSize)), 0, header, 8, 4);
            return header;
        }

        public virtual byte[] GetData()
        {
            return _packetData;
        }

        public virtual byte[] ToByteArray()
        {
            var data = GetData();
            var header = GetHeader(data.Length);
            var arr = new byte[header.Length + data.Length];
            Array.Copy(header, 0, arr, 0, header.Length);
            Array.Copy(data, 0, arr, header.Length, data.Length);
            return arr;
        }
    }
}
