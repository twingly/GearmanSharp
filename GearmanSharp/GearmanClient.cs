using System;
using System.IO;
using System.Linq;
using System.Text;
using Twingly.Gearman.Configuration;
using Twingly.Gearman.Exceptions;

namespace Twingly.Gearman
{
    public class GearmanClient : GearmanConnectionManager
    {
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

            foreach (var connection in GetAliveConnections())
            {
                try
                {
                    var protocol = new GearmanClientProtocol(connection);
                    var result = protocol.SubmitJob(functionName, argumentSerializer(functionArgument), uniqueId, priority);
                    return result == null ? null : resultDeserializer(result);
                }
                catch (GearmanConnectionException)
                {
                    connection.MarkAsDead();
                }
            }

            throw new NoServerAvailableException("Failed to submit job, no job server available");
        }

        public string SubmitBackgroundJob(string functionName, byte[] functionArgument)
        {
            return SubmitBackgroundJob(functionName, functionArgument, CreateRandomUniqueId(), GearmanJobPriority.Normal);
        }

        public string SubmitBackgroundJob(string functionName, byte[] functionArgument, string uniqueId, GearmanJobPriority priority)
        {
            return SubmitBackgroundJob<byte[]>(functionName, functionArgument, uniqueId, GearmanJobPriority.Normal, data => (data));
        }

        public string SubmitBackgroundJob<TArg>(string functionName, TArg functionArgument,
            DataSerializer<TArg> argumentSerializer)
            where TArg : class
        {
            return SubmitBackgroundJob<TArg>(functionName, functionArgument, CreateRandomUniqueId(), GearmanJobPriority.Normal, argumentSerializer);
        }

        public string SubmitBackgroundJob<TArg>(string functionName, TArg functionArgument, string uniqueId, GearmanJobPriority priority,
            DataSerializer<TArg> argumentSerializer)
            where TArg : class
        {
            if (argumentSerializer == null)
                throw new ArgumentNullException("argumentSerializer");

            foreach (var connection in GetAliveConnections())
            {
                try
                {
                    var protocol = new GearmanClientProtocol(connection);
                    return protocol.SubmitBackgroundJob(functionName, argumentSerializer(functionArgument), uniqueId, priority);
                }
                catch (GearmanConnectionException)
                {
                    connection.MarkAsDead();
                }
            }

            throw new NoServerAvailableException("Failed to submit background job, no job server available");
        }
        
        private static string CreateRandomUniqueId()
        {
            // Guid with format "N" should be max 32 chars long (it won't write the hyphens).
            // We only really need something random here, so it's not that important that we use Guid really.
            // http://msdn.microsoft.com/en-us/library/97af8hh4.aspx
            return Guid.NewGuid().ToString("N");
        }
    }
}
