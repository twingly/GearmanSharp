using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman
{
    public delegate void GearmanJobFunction<TArg, TResult>(IGearmanJob<TArg, TResult> job)
        where TArg : class
        where TResult : class;
    
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

        public GearmanJobInfo Info { get; protected set; }
        public TArg FunctionArgument { get; protected set; }

        public GearmanJob(GearmanWorkerProtocol protocol, GearmanJobInfo jobAssignment,
            DataDeserializer<TArg> argumentDeserializer, DataSerializer<TResult> resultSerializer)
        {
            _serializer = resultSerializer;
            _deserializer = argumentDeserializer;
            _protocol = protocol;
            Info = jobAssignment;
            FunctionArgument = _deserializer(jobAssignment.FunctionArgument);
        }

        public void Complete()
        {
            _protocol.WorkComplete(Info.JobHandle);
        }

        public void Complete(TResult result)
        {
            _protocol.WorkComplete(Info.JobHandle, _serializer(result));
        }

        public void Fail()
        {
            _protocol.WorkFail(Info.JobHandle);
        }

        public void SetStatus(uint numerator, uint denominator)
        {
            _protocol.WorkStatus(Info.JobHandle, numerator, denominator);
        }
    }
}