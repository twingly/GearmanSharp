using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;

namespace Twingly.Gearman
{
    /// <summary>
    /// Represents an abstract Socket (that can be mocked).
    /// </summary>
    public interface ISocket
    {
        bool Connected { get; }

        void Connect(string host, int port);
        int Send(byte[] buffer);
        int Receive(byte[] buffer, int size, SocketFlags socketFlags);
        int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags);
        void Shutdown();
        void Close();
    }
}