using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twingly.Gearman.Packets
{
    public class JobCreatedResponse : ResponsePacket
    {
        public string JobHandle { get; protected set; }

        public JobCreatedResponse(byte[] packetData) : base(PacketType.JOB_CREATED, packetData)
        {
            string handle;
            ParseString(packetData, 0, out handle);
            JobHandle = handle;
        }
    }
}
