using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Twingly.Gearman.Exceptions
{
    /// <summary>
    /// Represents an exception that occured in a registered job function when processing a Gearman job.
    /// </summary>
    public class GearmanFunctionInternalException : GearmanException
    {
        public JobAssignment JobAssignment { get; set; }

        public GearmanFunctionInternalException(JobAssignment jobAssignment)
        {
            JobAssignment = jobAssignment;
        }

        public GearmanFunctionInternalException(JobAssignment jobAssignment, string message)
            : base(message)
        {
            JobAssignment = jobAssignment;
        }

        public GearmanFunctionInternalException(JobAssignment jobAssignment, string message, Exception innerException)
            : base(message, innerException)
        {
            JobAssignment = jobAssignment;
        }

        protected GearmanFunctionInternalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
