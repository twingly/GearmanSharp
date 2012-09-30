using System;

namespace Twingly.Gearman
{
    public interface IGearmanClientEventHandler
    {
        event EventHandler JobCreated;
        event EventHandler<GearmanJobData> JobCompleted;
        event EventHandler JobFailed;
        event EventHandler<GearmanJobData> JobData;
        event EventHandler<GearmanJobData> JobWarning;
        event EventHandler<GearmanJobStatus> JobStatus;
        event EventHandler<GearmanJobData> JobException;
    }
}
