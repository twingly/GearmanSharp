using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twingly.Gearman.Exceptions;

namespace Twingly.Gearman.Packets
{
    public class SubmitJobRequest : RequestPacket
    {
        public string FunctionName { get; protected set; }
        public byte[] FunctionArgument { get; protected set; }
        public string UniqueId { get; protected set; }

        public SubmitJobRequest(string functionName, byte[] functionArgument, bool background, string uniqueId, GearmanJobPriority priority)
            : base(GetType(priority, background))
        {
            if (functionName == null)
                throw new ArgumentNullException("functionName");

            FunctionName = functionName;
            FunctionArgument = functionArgument ?? new byte[0];
            UniqueId = uniqueId ?? "";
        }

        private static PacketType GetType(GearmanJobPriority priority, bool background)
        {
            switch (priority)
            {
                case GearmanJobPriority.High:
                    return background ? PacketType.SUBMIT_JOB_HIGH_BG : PacketType.SUBMIT_JOB_HIGH;
                case GearmanJobPriority.Normal:
                    return background ? PacketType.SUBMIT_JOB_BG : PacketType.SUBMIT_JOB;
                case GearmanJobPriority.Low:
                    return background ? PacketType.SUBMIT_JOB_LOW_BG : PacketType.SUBMIT_JOB_LOW;
                default:
                    throw new GearmanApiException("Unknown priority and background combination for SubmitJobRequest");
            }
        }

        public override byte[] GetData()
        {
            return Util.JoinByteArraysForData(
                Encoding.UTF8.GetBytes(FunctionName),
                Encoding.UTF8.GetBytes(UniqueId),
                FunctionArgument);
        }
    }
}
