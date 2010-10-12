using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using Twingly.Gearman.Configuration;
using Twingly.Gearman.Exceptions;

namespace Twingly.Gearman
{
    public abstract class GearmanConnectionManager
    {
        private const int _DEFAULT_PORT = 4730;

        private readonly IList<IGearmanConnection> _connections;
        private IGearmanConnectionFactory _connectionFactory;

        public IGearmanConnectionFactory ConnectionFactory
        {
            get
            {
                return _connectionFactory;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _connectionFactory = value;
            }
        }

        protected GearmanConnectionManager()
        {
            _connections = new List<IGearmanConnection>();
            _connectionFactory = new GearmanConnectionFactory();
        }

        protected GearmanConnectionManager(string clusterName)
            : this()
        {
            if (clusterName == null)
                throw new ArgumentNullException("clusterName");

            var section = ConfigurationManager.GetSection("gearman") as GearmanConfigurationSection;
            if (section == null)
                throw new ConfigurationErrorsException("Section gearman is not found.");

            ParseConfiguration(section.Clusters[clusterName]);
        }

        protected GearmanConnectionManager(ClusterConfigurationElement clusterConfiguration)
            : this()
        {
            if (clusterConfiguration == null)
                throw new ArgumentNullException("clusterConfiguration");

            ParseConfiguration(clusterConfiguration);
        }

        private void ParseConfiguration(ClusterConfigurationElement cluster)
        {
            foreach (ServerConfigurationElement server in cluster.Servers)
            {
                AddServer(server.Host, server.Port);
            }
        }

        public void AddServer(string host)
        {
            AddServer(host, _DEFAULT_PORT);
        }

        public void AddServer(string host, int port)
        {
            AddConnection(ConnectionFactory.CreateConnection(host, port));
        }

        public void DisconnectAll()
        {
            foreach (var connection in _connections)
            {
                connection.Disconnect();
            }
        }

        private void AddConnection(IGearmanConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            _connections.Add(connection);
        }

        protected IEnumerable<IGearmanConnection> GetAliveConnections()
        {
            var connections = _connections.Shuffle(new Random()).ToList();
            var isAllDead = _connections.Where(conn => conn.IsDead()).Count() == _connections.Count;
            var aliveConnections = new List<IGearmanConnection>();

            foreach (var connection in connections)
            {
                // Try to reconnect if they're not connected and not dead, or if all servers are dead, we will try to reconnect them anyway.
                if (!connection.IsConnected() && (!connection.IsDead() || isAllDead))
                {
                    // Should we catch exception here? What is the typical use case of this function?
                    // Perhaps it's more useful to actually catch (all?) exceptions here so that a worker or
                    // client can have one of many connections fail. If this will throw every time a server
                    // goes down, doesn't that defeat the point of having multiple servers to gain availability?
                    try
                    {
                        connection.Connect();
                        OnConnectionConnected(connection);

                        // quick idea: Make GearmanConnection a base class and sub class it differently for the
                        // client and the worker, where the worker always registers all functions when connecting?
                        // Could that work?
                    }
                    catch (Exception)
                    {
                        // Perhaps wrap all connection related exceptions in so we can catch something
                        // more specific?
                        continue;
                    }
                }

                if (connection.IsConnected())
                {
                    aliveConnections.Add(connection);
                }
            }

            return aliveConnections;
        }

        protected virtual void OnConnectionConnected(IGearmanConnection connection)
        {
        }
    }
}