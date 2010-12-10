using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Twingly.Gearman.Exceptions;
using Twingly.Gearman.Packets;

namespace Twingly.Gearman
{
    public class GearmanConnection : IGearmanConnection
    {
        public const int DEFAULT_SEND_TIMEOUT_MILLISECONDS = 3*1000;
        public const int DEFAULT_RECEIVE_TIMEOUT_MILLISECONDS = 60*1000;

        private readonly TimeSpan _deadServerRetryInterval = TimeSpan.FromSeconds(60); // TODO: make configurable

        private ISocket _socket;
        private bool _isDead;
        private DateTime _nextRetry;

        /// <summary>
        /// The send timeout is used to determine how long the client should wait for data to be sent.
        /// and received from the server, specified in milliseconds. The default value is DEFAULT_SEND_TIMEOUT_MILLISECONDS.
        /// </summary>
        public int SendTimeout { get; set; }

        /// <summary>
        /// The receive timeout is used to determine how long the client should wait for data to be received from the server,
        /// specified in milliseconds. The default value is DEFAULT_RECEIVE_TIMEOUT_MILLISECONDS.
        /// </summary>
        public int ReceiveTimeout { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }

        public GearmanConnection(string host, int port)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            Host = host;
            Port = port;
            SendTimeout = DEFAULT_SEND_TIMEOUT_MILLISECONDS;
            ReceiveTimeout = DEFAULT_RECEIVE_TIMEOUT_MILLISECONDS;
            _isDead = false;
        }

        public bool IsDead()
        {
            if (_isDead && DateTime.Now >= _nextRetry)
                _isDead = false;

            return _isDead;
        }

        public void MarkAsDead()
        {
            Disconnect();
            _isDead = true;
            _nextRetry = DateTime.Now + _deadServerRetryInterval;
        }

        public void Connect()
        {
            if (IsConnected())
                return;

            Close();

            try
            {
                _socket = new SocketAdapter(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = true,
                    ReceiveTimeout = ReceiveTimeout,
                    SendTimeout = SendTimeout
                });

                _socket.Connect(Host, Port);
            }
            catch (Exception ex)
            {
                throw new GearmanConnectionException("Could not connect", ex);
            }

            if (!_socket.Connected)
            { 
                throw new GearmanConnectionException("Socket not connected");
            }
            
            _isDead = false;
        }

        public void Disconnect()
        {
            Close();
        }

        public void SendPacket(RequestPacket p)
        {
            try
            {
                _socket.Send(p.ToByteArray());
            }
            catch (Exception e)
            {
                new GearmanConnectionException("Unable to send packet", e);
            }
        }

        public IResponsePacket GetNextPacket()
        {
            var header = new byte[12];
            var packetMagic = new byte[4];
            byte[] packetData;
            try
            {
                _socket.Receive(header, 12, SocketFlags.None);
                Array.Copy(header, 0, packetMagic, 0, 4);

                if (!packetMagic.SequenceEqual(ResponsePacket.Magic))
                    throw new GearmanApiException("Response packet magic does not match");
                
                var packetType = (PacketType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 4));
                int packetSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 8));

                packetData = new byte[packetSize];
                if (packetSize > 0)
                {
                    int bytesRead = 0;
                    do
                    {
                        bytesRead += _socket.Receive(packetData, bytesRead, packetSize - bytesRead, SocketFlags.None);
                    } while (bytesRead < packetSize);
                }

                return ResponsePacket.Create(packetType, packetData);
            }
            catch (Exception e)
            {
                throw new GearmanConnectionException("Error reading data from socket", e);
            }
        }

        private void Close()
        {
            if (_socket != null)
            {
                try
                {
                    _socket.Shutdown();
                    _socket.Close();
                }
                catch (Exception)
                {
                    //logger.Error("Error shutting down and closing socket: " + EndPoint, e);
                }
                finally
                {
                    _socket = null;
                }
            }
        }
        /// <summary>
        /// Checks if the underlying socket and stream is connected and available.
        /// </summary>
        public bool IsConnected()
        {
            return _socket != null && _socket.Connected;
        }
    }
}