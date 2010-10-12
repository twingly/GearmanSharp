using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Twingly.Gearman.Packets
{
    public class WorkFailRequest : RequestPacket
    {
        public string JobHandle { get; protected set; }

        public WorkFailRequest(string jobHandle) : base(PacketType.WORK_FAIL)
        {
            if (jobHandle == null)
                throw new ArgumentNullException("jobHandle");

            JobHandle = jobHandle;
        }

        public override byte[] GetData()
        {
            return Util.JoinByteArraysForData(Encoding.UTF8.GetBytes(JobHandle));
        }
    }
}