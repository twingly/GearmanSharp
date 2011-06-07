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
        private const int _NO_JOB_SLEEP_TIME_MS = 1000;
        private const int _NO_SERVERS_SLEEP_TIME_MS = 1000;

        protected volatile bool ContinueWorking = false;
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

        public void StartWorkLoop()
        {
            ContinueWorking = true;
            _resetEvent.Reset();
            _workLoopThread.Start();
        }

        public void StopWorkLoop()
        {
            ContinueWorking = false;
            _resetEvent.Set();
            if (_workLoopThread.IsAlive)
            {
                _workLoopThread.Join();
            }
        }

        /// <summary>
        /// Called when a job function throws an exception. Does nothing and returns false, to not abort the work loop.
        /// </summary>
        /// <param name="exception">The exception thrown by the job function.</param>
        /// <param name="jobAssignment">The job assignment that the job function got.</param>
        /// <returns>Return true if it should throw, or false if it should not throw after the return.</returns>
        protected override bool OnJobException(Exception exception, GearmanJobInfo jobAssignment)
        {
            // Don't throw the exception, as that would abort the work loop.
            return false;
        }

        private void WorkLoopThreadProc()
        {
            var noJobCount = 0;
            while (ContinueWorking)
            {
                try
                {
                    var aliveConnections = GetAliveConnections();

                    if (aliveConnections.Count() < 1)
                    {
                        // No servers available, sleep for a while and try again later
                        _resetEvent.WaitOne(_NO_SERVERS_SLEEP_TIME_MS, false);
                        _resetEvent.Reset();
                        noJobCount = 0;
                    }
                    else
                    {
                        foreach (var connection in aliveConnections)
                        {
                            if (!ContinueWorking)
                            {
                                break;
                            }

                            var didWork = Work(connection);
                            noJobCount = didWork ? 0 : noJobCount + 1;
                        }

                        if (noJobCount >= _NO_JOB_COUNT_BEFORE_SLEEP)
                        {
                            _resetEvent.WaitOne(_NO_JOB_SLEEP_TIME_MS, false);
                            _resetEvent.Reset();
                            noJobCount = 0;
                        }
                    }
                }
                catch (Exception)
                {
                    ContinueWorking = false;
                    throw;
                }
            }
        }
    }
}