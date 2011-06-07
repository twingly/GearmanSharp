using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman
{
    public interface IGearmanJob<TArg, TResult>
    {
        GearmanJobInfo Info { get; }

        /// <summary>
        /// The deserialized function argument.
        /// </summary>
        TArg FunctionArgument { get; }

        void Complete();
        void Complete(TResult result);

        void Fail();

        // Using GEARMAND_COMMAND_WORK_EXCEPTION is not recommended at time of this writing
        // http://groups.google.com/group/gearman/browse_thread/thread/5c91acc31bd10688/529e586405ed37fe
        //
        //void Exception();
        //void Exception(byte[] exception);
        
        void SetStatus(uint numerator, uint denominator);
    }
}