using System;
using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman
{
    public class GearmanJobStatus : EventArgs
    {
        public string JobHandle { get; protected set; }
        public bool IsKnown { get; protected set; }
        public bool IsRunning { get; protected set; }
        public uint CompletionNumerator { get; protected set; }
        public uint CompletionDenominator { get; protected set; }
        
        public GearmanJobStatus(string jobHandle, bool isKnown, bool isRunning, uint completionNumerator, uint completionDenominator)
        {
            JobHandle = jobHandle;
            IsKnown = isKnown;
            IsRunning = isRunning;
            CompletionNumerator = completionNumerator;
            CompletionDenominator = completionDenominator;
        }

        public double GetCompletionPercent()
        {
            return CompletionNumerator / (double)CompletionDenominator;
        }
    }
}