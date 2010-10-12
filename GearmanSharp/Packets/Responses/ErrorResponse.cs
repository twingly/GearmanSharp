using System;
using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman.Packets
{
    public class ErrorResponse : ResponsePacket
    {
        public string ErrorCode { get; protected set; }
        public string ErrorText { get; protected set; }

        public ErrorResponse(byte[] packetData) : base(PacketType.ERROR, packetData)
        {
            string errorCode;
            string errorText;
            var nextOffset = ParseString(packetData, 0, out errorCode);
            ErrorCode = errorCode;

            ParseString(packetData, nextOffset, out errorText);
            ErrorText = errorText;
        }
    }
}