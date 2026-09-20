using Lab2.SimulationNodes;

namespace Lab2.SimulationHandlers;

public class SimulationMetricsCollector(int p1DevCount, int p2DevCount, int p3DevCount, Process p1, Process p2, Process p3)
{
    private double _p1QueueSum, _p2QueueSum, _p3QueueSum;
    private double _p1BusySum, _p2BusySum, _p3BusySum;
    private long _sampleCount;

    public async Task StartCollectingAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _p1QueueSum += p1.QueueLength;
            _p2QueueSum += p2.QueueLength;
            _p3QueueSum += p3.QueueLength;

            _p1BusySum += p1.Statistics.GetSnapshot().utilization * p1DevCount;
            _p2BusySum += p2.Statistics.GetSnapshot().utilization * p2DevCount;
            _p3BusySum += p3.Statistics.GetSnapshot().utilization * p3DevCount;
            _sampleCount++;

            await Task.Delay(100, cancellationToken);
        }
    }

    public double GetP1AvgQueue() => _sampleCount > 0 ? Math.Round(_p1QueueSum / _sampleCount, 2) : 0;
    public double GetP1AvgBusy(int devs) => _sampleCount > 0 ? Math.Round(_p1BusySum / _sampleCount, 2) : 0;
    public double GetP2AvgQueue() => _sampleCount > 0 ? Math.Round(_p2QueueSum / _sampleCount, 2) : 0;
    public double GetP2AvgBusy(int devs) => _sampleCount > 0 ? Math.Round(_p2BusySum / _sampleCount, 2) : 0;
    public double GetP3AvgQueue() => _sampleCount > 0 ? Math.Round(_p3QueueSum / _sampleCount, 2) : 0;
    public double GetP3AvgBusy(int devs) => _sampleCount > 0 ? Math.Round(_p3BusySum / _sampleCount, 2) : 0;
}