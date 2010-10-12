using System;
using System.Runtime.Serialization;

namespace Twingly.Gearman.Exceptions
{
    public class NoServerAvailableException : GearmanException
    {
        public NoServerAvailableException() { }
        public NoServerAvailableException(string message) : base(message) { }
        public NoServerAvailableException(string message, Exception innerException) : base(message, innerException) { }
        protected NoServerAvailableException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}