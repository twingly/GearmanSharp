using System;
using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman.Packets
{
    public class GrabJobRequest : RequestPacket
    {
        public GrabJobRequest() : base(PacketType.GRAB_JOB)
        {
        }

        public override byte[] GetData()
        {
            return new byte[0];
        }
    }
}