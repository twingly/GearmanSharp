using System;
using System.IO;
using System.Linq;
using System.Text;
using Twingly.Gearman.Configuration;
using Twingly.Gearman.Exceptions;

namespace Twingly.Gearman
{
    public class GearmanClient : GearmanConnectionManager, IGearmanClient, IGearmanClientEventHandler
    {
        public event EventHandler JobCreated;
        public event EventHandler<GearmanJobData> JobCompleted;
        public event EventHandler JobFailed;
        public event EventHandler<GearmanJobData> JobData;
        public event EventHandler<GearmanJobData> JobWarning;
        public event EventHandler<GearmanJobStatus> JobStatus;
        public event EventHandler<GearmanJobData> JobException;

        public GearmanClient()
        {
        }

        public GearmanClient(string clusterName)
            : base(clusterName)
        {
        }

        public GearmanClient(ClusterConfigurationElement clusterConfiguration)
            : base(clusterConfiguration)
        {
        }

        public GearmanJobStatus GetStatus(GearmanJobRequest jobRequest)
        {
            return new GearmanClientProtocol(jobRequest.Connection).GetStatus(jobRequest.JobHandle);
        }

        public byte[] SubmitJob(string functionName, byte[] functionArgument)
        {
            return SubmitJob(functionName, functionArgument, Guid.NewGuid().ToString(), GearmanJobPriority.Normal);
        }

        public byte[] SubmitJob(string functionName, byte[] functionArgument, string uniqueId, GearmanJobPriority priority)
        {
            return SubmitJob<byte[], byte[]>(functionName, functionArgument, Guid.NewGuid().ToString(), GearmanJobPriority.Normal,
                data => (data), data => (data));
        }

        public TResult SubmitJob<TArg, TResult>(string functionName, TArg functionArgument,
            DataSerializer<TArg> argumentSerializer, DataDeserializer<TResult> resultDeserializer)
            where TArg : class
            where TResult : class
        {
            return SubmitJob<TArg, TResult>(functionName, functionArgument, Guid.NewGuid().ToString(), GearmanJobPriority.Normal,
                argumentSerializer, resultDeserializer);
        }

        public TResult SubmitJob<TArg, TResult>(string functionName, TArg functionArgument, string uniqueId, GearmanJobPriority priority,
            DataSerializer<TArg> argumentSerializer, DataDeserializer<TResult> resultDeserializer)
            where TArg : class
            where TResult : class
        {
            if (argumentSerializer == null)
                throw new ArgumentNullException("argumentSerializer");

            if (resultDeserializer == null)
                throw new ArgumentNullException("resultDeserializer");

            var functionArgumentBytes = argumentSerializer(functionArgument); // Do this before calling SendClientCommand.
            var result = SendClientCommand(protocol => protocol.SubmitJob(
                functionName,
                functionArgumentBytes,
                uniqueId,
                priority));
            return result == null ? null : resultDeserializer(result);
        }

        public GearmanJobRequest SubmitBackgroundJob(string functionName, byte[] functionArgument)
        {
            return SubmitBackgroundJob(functionName, functionArgument, CreateRandomUniqueId(), GearmanJobPriority.Normal);
        }

        public GearmanJobRequest SubmitBackgroundJob(string functionName, byte[] functionArgument, string uniqueId, GearmanJobPriority priority)
        {
            return SubmitBackgroundJob<byte[]>(functionName, functionArgument, uniqueId, GearmanJobPriority.Normal, data => (data));
        }

        public GearmanJobRequest SubmitBackgroundJob<TArg>(string functionName, TArg functionArgument,
            DataSerializer<TArg> argumentSerializer)
            where TArg : class
        {
            return SubmitBackgroundJob<TArg>(functionName, functionArgument, CreateRandomUniqueId(), GearmanJobPriority.Normal, argumentSerializer);
        }

        public GearmanJobRequest SubmitBackgroundJob<TArg>(string functionName, TArg functionArgument, string uniqueId, GearmanJobPriority priority,
            DataSerializer<TArg> argumentSerializer)
            where TArg : class
        {
            if (argumentSerializer == null)
                throw new ArgumentNullException("argumentSerializer");

            var functionArgumentBytes = argumentSerializer(functionArgument); // Do this before calling SendClientCommand.
            return SendClientCommand(protocol => SubmitBackgroundJob(protocol, functionName, functionArgumentBytes, uniqueId, priority));
        }

        private static GearmanJobRequest SubmitBackgroundJob(GearmanClientProtocol protocol, string functionName, byte[] functionArgument,
            string uniqueId, GearmanJobPriority priority)
        {
            var jobHandle = protocol.SubmitBackgroundJob(
                functionName,
                functionArgument,
                uniqueId,
                priority);
            return new GearmanJobRequest(protocol.Connection, jobHandle);
        }

        protected T SendClientCommand<T>(Func<GearmanClientProtocol,T> commandFunc)
        {
            foreach (var connection in GetAliveConnections())
            {
                try
                {
                    GearmanClientProtocol proto = new GearmanClientProtocol(connection);
                    // Pass through all events
                    proto.JobCompleted += (o, e) => onJobCompleted(e);
                    proto.JobCreated += (o, e) => onJobCreated(e);
                    proto.JobData += (o, e) => onJobData(e);
                    proto.JobException += (o, e) => onJobException(e);
                    proto.JobFailed += (o, e) => onJobFailed(e);
                    proto.JobStatus += (o, e) => onJobStatus(e);
                    proto.JobWarning += (o, e) => onJobWarning(e);
                    return commandFunc(proto);
                }
                catch (GearmanConnectionException)
                {
                    connection.MarkAsDead();
                }
            }

            throw new NoServerAvailableException("Failed to send command, no job server available"); 
        }
        
        private static string CreateRandomUniqueId()
        {
            // Guid with format "N" should be max 32 chars long (it won't write the hyphens).
            // We only really need something random here, so it's not that important that we use Guid really.
            // http://msdn.microsoft.com/en-us/library/97af8hh4.aspx
            return Guid.NewGuid().ToString("N");
        }

        protected void onJobCreated(EventArgs e)
        {
            EventHandler handler = JobCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void onJobCompleted(GearmanJobData e) {
            EventHandler<GearmanJobData> handler = JobCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void onJobFailed(EventArgs e)
        {
            EventHandler handler = JobFailed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void onJobData(GearmanJobData e)
        {
            EventHandler<GearmanJobData> handler = JobData;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void onJobWarning(GearmanJobData e)
        {
            EventHandler<GearmanJobData> handler = JobWarning;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void onJobStatus(GearmanJobStatus e)
        {
            EventHandler<GearmanJobStatus> handler = JobStatus;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void onJobException(GearmanJobData e)
        {
            EventHandler<GearmanJobData> handler = JobException;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
