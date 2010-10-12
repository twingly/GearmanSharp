using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Twingly.Gearman.Exceptions
{
    /// <summary>
    /// Represents unexpected errors/exceptions that occured in a Gearman function when processing a Gearman job.
    /// </summary>
    public class GearmanFunctionInternalException : GearmanException
    {
        public GearmanFunctionInternalException() { }
        public GearmanFunctionInternalException(string message) : base(message) { }
        public GearmanFunctionInternalException(string message, Exception innerException) : base(message, innerException) { }
        protected GearmanFunctionInternalException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
