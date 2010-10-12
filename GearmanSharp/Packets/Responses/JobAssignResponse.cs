using System;
using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman.Packets
{
    public interface IJobAssignResponse : IResponsePacket
    {
        string JobHandle { get; }
        string FunctionName { get; }
        byte[] FunctionArgument { get; }
    }

    public class JobAssignResponse : ResponsePacket, IJobAssignResponse
    {
        public string JobHandle { get; protected set; }
        public string FunctionName { get; protected set; }
        public byte[] FunctionArgument { get; protected set; }

        public JobAssignResponse(byte[] packetData) : base(PacketType.JOB_ASSIGN, packetData)
        {
            string jobHandle;
            string functionName;
            var functionNameOffset = ParseString(packetData, 0, out jobHandle);
            JobHandle = jobHandle;

            var functionArgumentOffset = ParseString(packetData, functionNameOffset, out functionName);
            FunctionName = functionName;

            var functionArgumentSize = (packetData.Length - functionArgumentOffset);
            var functionArgument = new byte[functionArgumentSize];
            Array.Copy(packetData, functionArgumentOffset, functionArgument, 0, functionArgumentSize);
            FunctionArgument = functionArgument;
        }
    }
}