using System.Diagnostics;

namespace Lab2.Logging;

public class DeviceStatistics(int channelsCount)
{
    private readonly Stopwatch _uptimeStopwatch = Stopwatch.StartNew();
    private double _totalBusyTimeSeconds;
    private long _processedCount;
    private readonly Lock _lock = new();

    public void RecordWork(double serviceTime)
    {
        lock (_lock)
        {
            _totalBusyTimeSeconds += serviceTime;
            _processedCount++;
        }
    }

    public (double utilization, long processedCount, double uptimeSeconds) GetSnapshot()
    {
        lock (_lock)
        {
            var totalTime = _uptimeStopwatch.Elapsed.TotalSeconds;
            var maxPossibleWorkTime = totalTime * channelsCount;
            var utilization = maxPossibleWorkTime > 0
                ? Math.Min(1.0, _totalBusyTimeSeconds / maxPossibleWorkTime)
                : 0;

            return (utilization, _processedCount, totalTime);
        }
    }
}
