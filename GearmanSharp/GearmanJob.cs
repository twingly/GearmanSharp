using System;
using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman
{
    public enum GearmanJobPriority
    {
        High = 1,
        Normal = 2,
        Low = 3
    };

    public class GearmanJob<TArg, TResult> : IGearmanJob<TArg, TResult>
        where TArg : class
        where TResult : class
    {
        private readonly DataSerializer<TResult> _serializer;
        private readonly DataDeserializer<TArg> _deserializer;
        private readonly GearmanWorkerProtocol _protocol;

        public string JobHandle { get; protected set; }
        public string FunctionName { get; protected set; }
        public TArg FunctionArgument { get; protected set; }

        public GearmanJob(GearmanWorkerProtocol protocol, JobAssignment jobAssignment,
            DataDeserializer<TArg> argumentDeserializer, DataSerializer<TResult> resultSerializer)
        {
            _serializer = resultSerializer;
            _deserializer = argumentDeserializer;
            _protocol = protocol;
            JobHandle = jobAssignment.JobHandle;
            FunctionName = jobAssignment.FunctionName;
            FunctionArgument = _deserializer(jobAssignment.FunctionArgument);
        }

        public void Complete()
        {
            _protocol.WorkComplete(JobHandle);
        }

        public void Complete(TResult result)
        {
            _protocol.WorkComplete(JobHandle, _serializer(result));
        }

        public void Fail()
        {
            _protocol.WorkFail(JobHandle);
        }
    }
}