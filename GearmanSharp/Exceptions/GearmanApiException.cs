using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Twingly.Gearman.Exceptions
{
    public class GearmanApiException : GearmanException
    {
        public GearmanApiException() { }
        public GearmanApiException(string message) : base(message) { }
        public GearmanApiException(string message, Exception innerException) : base(message, innerException) { }
        protected GearmanApiException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}