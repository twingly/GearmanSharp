using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman
{
    public class JobAssignment
    {
        public string JobHandle;
        public string FunctionName;
        public byte[] FunctionArgument;
    }
}