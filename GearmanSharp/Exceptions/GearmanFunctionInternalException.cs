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
        public GearmanJobInfo JobInfo { get; set; }

        public GearmanFunctionInternalException(GearmanJobInfo jobAssignment)
        {
            JobInfo = jobAssignment;
        }

        public GearmanFunctionInternalException(GearmanJobInfo jobAssignment, string message)
            : base(message)
        {
            JobInfo = jobAssignment;
        }

        public GearmanFunctionInternalException(GearmanJobInfo jobAssignment, string message, Exception innerException)
            : base(message, innerException)
        {
            JobInfo = jobAssignment;
        }

        protected GearmanFunctionInternalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
