using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman
{
    public interface IGearmanConnectionFactory
    {
        IGearmanConnection CreateConnection(string host, int port);
    }
}