using System.Net.Sockets;

namespace Twingly.Gearman
{
    /// <summary>
    /// Adapter for System.Net.Sockets.Socket that implements ISocket.
    /// Passes all calls to the underlying Socket.
    /// AddressFamily.InterNetwork. SocketType.Stream. ProtocolType.Tcp.
    /// </summary>
    public class SocketAdapter : ISocket
    {
        private readonly Socket _socket;

        public SocketAdapter(Socket socket)
        {
            _socket = socket;
        }

        public virtual bool Connected
        {
            get { return _socket.Connected; }
        }

        public virtual void Connect(string host, int port)
        {
            _socket.Connect(host, port);
        }

        public virtual int Send(byte[] buffer)
        {
            return _socket.Send(buffer);
        }

        public virtual int Receive(byte[] buffer, int size, SocketFlags socketFlags)
        {
            return _socket.Receive(buffer, size, socketFlags);
        }

        public virtual int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            return _socket.Receive(buffer, offset, size, socketFlags);
        }

        public virtual void Shutdown()
        {
            _socket.Shutdown(SocketShutdown.Both);
        }

        public virtual void Close()
        {
            _socket.Close();
        }
    }
}