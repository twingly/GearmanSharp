using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Twingly.Gearman.Exceptions
{
    public class GearmanServerException : GearmanException
    {
        public string ErrorCode { get; set; }

        public GearmanServerException(string errorCode)
        {
            ErrorCode = errorCode;
        }

        public GearmanServerException(string errorCode, string errorText)
            : base(errorText)
        {
            ErrorCode = errorCode;
        }

        public GearmanServerException(string errorCode, string errorText, Exception innerException)
            : base(errorText, innerException)
        {
            ErrorCode = errorCode;
        }

        protected GearmanServerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}