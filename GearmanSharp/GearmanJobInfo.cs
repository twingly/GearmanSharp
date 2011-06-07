using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman
{
    public class GearmanJobInfo
    {
        public string JobHandle { get; set; }
        public string FunctionName { get; set; }
        public byte[] FunctionArgument { get; set; }
    }
}