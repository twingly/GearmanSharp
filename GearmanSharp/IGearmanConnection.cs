using System.Linq;
using System.Linq.Expressions;
using Twingly.Gearman.Packets;

namespace Twingly.Gearman
{
    public interface IGearmanConnection
    {
        void Connect();
        void Disconnect();
        void SendPacket(RequestPacket p);
        IResponsePacket GetNextPacket();

        bool IsConnected();

        string Host { get; }
        int Port { get; }

        bool IsDead(); // A dead connection should not be retried. When it's time to retry, it won't be dead.
        void MarkAsDead();
    }
}