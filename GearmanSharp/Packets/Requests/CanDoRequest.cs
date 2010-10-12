using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Twingly.Gearman.Packets
{
    public class CanDoRequest : RequestPacket
    {
        public string FunctionName { get; protected set; }

        public CanDoRequest(string functionName) : base(PacketType.CAN_DO)
        {
            if (functionName == null)
                throw new ArgumentNullException("functionName");

            FunctionName = functionName;
        }

        public override byte[] GetData()
        {
            return Util.JoinByteArraysForData(Encoding.UTF8.GetBytes(FunctionName));
        }
    }
}