using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Twingly.Gearman.Exceptions
{
    public class GearmanConnectionException : GearmanException
    {
        public GearmanConnectionException() { }
        public GearmanConnectionException(string message) : base(message) { }
        public GearmanConnectionException(string message, Exception innerException) : base(message, innerException) { }
        protected GearmanConnectionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}