using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Twingly.Gearman.Exceptions
{
    public class GearmanException : Exception
    {
        public GearmanException() { }
        public GearmanException(string message) : base(message) { }
        public GearmanException(string message, Exception innerException) : base(message, innerException) { }
        protected GearmanException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}