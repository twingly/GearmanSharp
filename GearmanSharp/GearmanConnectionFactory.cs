using System;
using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman
{
    public class GearmanConnectionFactory : IGearmanConnectionFactory
    {
        public IGearmanConnection CreateConnection(string host, int port)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            return new GearmanConnection(host, port);
        }
    }
}