using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Twingly.Gearman.Packets
{
    public class WorkCompleteRequest : RequestPacket
    {
        public string JobHandle { get; protected set; }
        public byte[] Result { get; protected set; }

        public WorkCompleteRequest(string jobHandle, byte[] result) : base(PacketType.WORK_COMPLETE)
        {
            if (jobHandle == null)
                throw new ArgumentNullException("jobHandle");

            JobHandle = jobHandle;
            Result = result;
        }

        public override byte[] GetData()
        {
            return Util.JoinByteArraysForData(Encoding.UTF8.GetBytes(JobHandle), Result);
        }
    }
}