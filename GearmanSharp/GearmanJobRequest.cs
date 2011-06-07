using System;
using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman
{
    public class GearmanJobRequest
    {
        public IGearmanConnection Connection { get; protected set; } // could this be an IGearmanConnection instead?

        public string JobHandle { get; set; }

        public GearmanJobRequest(IGearmanConnection connection, string jobHandle)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            if (jobHandle == null)
                throw new ArgumentNullException("jobHandle");

            Connection = connection;
            JobHandle = jobHandle;
        }
    }
}