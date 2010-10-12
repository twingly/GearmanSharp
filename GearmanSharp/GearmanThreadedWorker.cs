using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Twingly.Gearman.Configuration;

namespace Twingly.Gearman
{
    public class GearmanThreadedWorker : GearmanWorker
    {
        private const int _NO_JOB_COUNT_BEFORE_SLEEP = 10;
        private const int _NO_JOB_SLEEP_TIME = 1000;
        private const int _NO_SERVERS_SLEEP_TIME = 1000;

        private volatile bool _shouldQuit = false;
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);
        private readonly Thread _workLoopThread;

        public GearmanThreadedWorker()
        {
            _workLoopThread = new Thread(WorkLoopThreadProc);
        }

        public GearmanThreadedWorker(string clusterName)
            : base(clusterName)
        {
            _workLoopThread = new Thread(WorkLoopThreadProc);
        }

        public GearmanThreadedWorker(ClusterConfigurationElement clusterConfiguration)
            : base(clusterConfiguration)
        {
            _workLoopThread = new Thread(WorkLoopThreadProc);
        }

        private void WorkLoopThreadProc()
        {
            var noJobCount = 0;
            while (!_shouldQuit)
            {
                try
                {
                    var aliveConnections = GetAliveConnections();

                    if (aliveConnections.Count() < 1)
                    {
                        _resetEvent.WaitOne(_NO_SERVERS_SLEEP_TIME, false);
                        _resetEvent.Reset();
                        noJobCount = 0;
                    }
                    else
                    {
                        foreach (var connection in aliveConnections)
                        {
                            if (_shouldQuit)
                            {
                                break;
                            }

                            var didWork = Work(connection);
                            noJobCount = didWork ? 0 : noJobCount + 1;
                        }

                        if (noJobCount >= _NO_JOB_COUNT_BEFORE_SLEEP)
                        {
                            _resetEvent.WaitOne(_NO_JOB_SLEEP_TIME, false);
                            _resetEvent.Reset();
                            noJobCount = 0;
                        }
                    }
                }
                catch (Exception)
                {
                    _shouldQuit = true;
                    throw;
                }

            }
        }

        public void StartWorkLoop()
        {
            _shouldQuit = false;
            _resetEvent.Reset();
            _workLoopThread.Start();
        }

        public void StopWorkLoop()
        {
            _shouldQuit = true;
            _resetEvent.Set();
            if (_workLoopThread.IsAlive)
            {
                _workLoopThread.Join();
            }
        }
    }
}