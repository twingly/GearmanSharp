using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Twingly.Gearman.Configuration;
using Twingly.Gearman.Exceptions;

namespace Twingly.Gearman
{
    public delegate void GearmanJobFunction<TArg, TResult>(IGearmanJob<TArg, TResult> job) where TArg : class where TResult : class;

    public class GearmanWorker : GearmanConnectionManager
    {
        protected struct FunctionInformation
        {
            public Delegate ResultSerializer { get; set; }
            public Delegate ArgumentDeserializer { get; set; }
            public Delegate Function { get; set; }
            public Type ArgumentType { get; set; }
            public Type ResultType { get; set; }
            public ConstructorInfo JobConstructor { get; set; }
        }

        private string _clientId = null;
        private readonly IDictionary<string, FunctionInformation> _functionInformation = new Dictionary<string, FunctionInformation>();

        public GearmanWorker()
        {
        }

        public GearmanWorker(string clusterName)
            : base(clusterName)
        {
        }

        public GearmanWorker(ClusterConfigurationElement clusterConfiguration)
            : base(clusterConfiguration)
        {
        }

        public void SetClientId(string clientId)
        {
            if (clientId == null)
                throw new ArgumentNullException("clientId");

            _clientId = clientId;
            foreach (var connection in GetAliveConnections())
            {
                SetClientId(connection);
            }
        }

        public void RegisterFunction(string functionName, GearmanJobFunction<byte[], byte[]> function)
        {
            RegisterFunction(functionName, function, data => (data), data => (data));
        }

        public void RegisterFunction<TArg, TResult>(string functionName, GearmanJobFunction<TArg, TResult> function,
            DataDeserializer<TArg> argumentDeserializer, DataSerializer<TResult> resultSerializer)
            where TArg : class
            where TResult : class
        {
            if (functionName == null)
                throw new ArgumentNullException("functionName");

            if (function == null)
                throw new ArgumentNullException("function");

            if (resultSerializer == null)
                throw new ArgumentNullException("resultSerializer");

            if (argumentDeserializer == null)
                throw new ArgumentNullException("argumentDeserializer");

            AddFunction(functionName, function, resultSerializer, argumentDeserializer);

            foreach (var connection in GetAliveConnections())
            {
                RegisterFunction(connection, functionName);
            }
        }

        public bool Work()
        {
            var aliveConnections = GetAliveConnections();

            // TODO: What do we do if there are no alive servers?
            // When we call this function we only want to do one job and then it's interesting to know
            // if we didn't do any work because there weren't any, or because we didn't have any connections.
            if (aliveConnections.Count() < 1)
            {
                throw new NoServerAvailableException("No job servers");
            }

            if (aliveConnections.Count() > 0)
            {
                return Work(aliveConnections.First());
            }

            return false;
        }

        protected new virtual void OnConnectionConnected(IGearmanConnection connection)
        {
            RegisterAllFunctions(connection);
            SetClientId(connection);
        }

        private void SetClientId(IGearmanConnection connection)
        {
            new GearmanWorkerProtocol(connection).SetClientId(_clientId);
        }

        private void RegisterAllFunctions(IGearmanConnection connection)
        {
            foreach (var functionName in _functionInformation.Keys)
            {
                RegisterFunction(connection, functionName);
            }
        }

        private static void RegisterFunction(IGearmanConnection connection, string functionName)
        {
            new GearmanWorkerProtocol(connection).CanDo(functionName);
        }
        
        private void AddFunction<TArg, TResult>(string functionName, GearmanJobFunction<TArg, TResult> function,
            DataSerializer<TResult> resultSerializer, DataDeserializer<TArg> argumentDeserializer)
            where TArg : class
            where TResult : class 
        {
            var jobConstructorTypes = new Type[4]
            {
                typeof(GearmanWorkerProtocol),
                typeof(JobAssignment),
                typeof(DataDeserializer<TArg>),
                typeof(DataSerializer<TResult>)
            };

            var jobConstructorInfo = typeof(GearmanJob<TArg, TResult>).GetConstructor(jobConstructorTypes);

            if (jobConstructorInfo == null)
                throw new InvalidOperationException("Failed to locate the constructor for GearmanJob2<T>");

            _functionInformation.Add(functionName, new FunctionInformation
            {
                Function = function,
                ArgumentType = typeof(TArg),
                ResultType = typeof(TResult),
                ArgumentDeserializer = argumentDeserializer,
                ResultSerializer = resultSerializer,
                JobConstructor = jobConstructorInfo
            });
        }

        protected bool Work(IGearmanConnection connection)
        {
            var protocol = new GearmanWorkerProtocol(connection);
            var jobAssignment = protocol.GrabJob();

            if (jobAssignment == null)
                return false;

            if (!_functionInformation.ContainsKey(jobAssignment.FunctionName))
                throw new GearmanApiException(String.Format("Received work for unknown function {0}", jobAssignment.FunctionName));

            try
            {
                return CallFunction(protocol, jobAssignment);
            }
            catch (Exception)
            {
                // We failed to call the function and there isn't any good response to send the server.
                // According to this response on the mailing list, the best action is probably to close the connection:
                // "A worker disconnect with no response message is currently how the server's retry behavior is triggered."
                // http://groups.google.com/group/gearman/browse_thread/thread/5c91acc31bd10688/529e586405ed37fe
                // 
                // We can't send Complete or Fail for the job, because that would cause the job to be "done".
                connection.Disconnect();
                throw;
            }
            
        }

        private bool CallFunction(GearmanWorkerProtocol protocol, JobAssignment jobAssignment)
        {
            var functionInformation = _functionInformation[jobAssignment.FunctionName];

            object job;

            try
            {
                job = functionInformation.JobConstructor.Invoke(new object[]
                                                                    {
                                                                        protocol,                                   
                                                                        jobAssignment,
                                                                        functionInformation.ArgumentDeserializer,
                                                                        functionInformation.ResultSerializer,

                                                                    });
            }
            catch (Exception ex)
            {
                throw new GearmanException("Failed to invoke the GearmanJob constructor", ex);
            }

            try
            {
                functionInformation.Function.DynamicInvoke(job);
                return true;
            }
            catch (MemberAccessException ex)
            {
                throw new GearmanException("Failed to invoke the function dynamically", ex);
            }
            catch (TargetException ex)
            {
                throw new GearmanException("Failed to invoke the function dynamically", ex);
            }
            catch (TargetInvocationException ex)
            {
                throw new GearmanException("Failed to invoke the function dynamically", ex);
            }
            catch (Exception ex)
            {
                throw new GearmanFunctionInternalException(String.Format("Function '{0}' threw exception", jobAssignment.FunctionName), ex);
            }
        }
    }
}