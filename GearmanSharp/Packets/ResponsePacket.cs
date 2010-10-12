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

    public abstract class ResponsePacket : Packet, IResponsePacket
    {
        public static readonly byte[] Magic = new byte[4] { 0, (byte)'R', (byte)'E', (byte)'S' };

        protected byte[] RawPacketData { get; set; }

        protected ResponsePacket(PacketType packetType, byte[] packetData) : base(packetType)
        {
            RawPacketData = packetData;
        }

        public override byte[] GetMagic()
        {
            return Magic;
        }

        public override byte[] GetData()
        {
            return RawPacketData;
        }

        public static ResponsePacket Create(PacketType packetType, byte[] packetData)
        {
            switch (packetType)
            {
                case PacketType.JOB_CREATED:
                    return new JobCreatedResponse(packetData);

                case PacketType.WORK_DATA:
                    return new WorkDataResponse(packetData);

                case PacketType.WORK_WARNING:
                    return new WorkWarningResponse(packetData);

                case PacketType.WORK_STATUS:
                    return new WorkStatusResponse(packetData);

                case PacketType.WORK_COMPLETE:
                    return new WorkCompleteResponse(packetData);

                case PacketType.WORK_FAIL:
                    return new WorkFailResponse(packetData);

                case PacketType.WORK_EXCEPTION:
                    return new WorkExceptionResponse(packetData);

                case PacketType.STATUS_RES:
                    throw new NotImplementedException();

                case PacketType.OPTION_RES:
                    throw new NotImplementedException();


                /* Client and worker response packets */
                case PacketType.ECHO_RES:
                    throw new NotImplementedException();

                case PacketType.ERROR:
                    return new ErrorResponse(packetData);


                /* Worker response packets */
                case PacketType.NOOP:
                    return new NoopResponse(packetData);

                case PacketType.NO_JOB:
                    return new NoJobResponse(packetData);

                case PacketType.JOB_ASSIGN:
                    return new JobAssignResponse(packetData);

                case PacketType.JOB_ASSIGN_UNIQ:
                    throw new NotImplementedException();

                default:
                    throw new GearmanApiException("Unknown packet type");
            }
        }
        
        public static int ParseString(byte[] data, int offset, out string str)
        {
            return ParseString(data, offset, out str, Encoding.UTF8);
        }

        public static int ParseString(byte[] data, int startIndex, out string str, Encoding encoding)
        {
            int offset = startIndex;
            for (; offset < data.Length && data[offset] != 0; offset++) { }
            str = encoding.GetString(data.Slice(startIndex, offset));

            return offset + 1; // next position
        }
    }
}