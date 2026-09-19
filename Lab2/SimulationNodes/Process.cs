using Lab2.Distributions;
using Lab2.Logging;
using Lab2.SimulationUtils;
using System.Collections.Concurrent;

namespace Lab2.SimulationNodes;

public class Process(IDistributionStrategy distribution, string name, int channelsCount, int queueLimit)
    : ParametrizedNode(distribution), IReceiverNode
{
    public Process(double delay, string name, int channelsCount, int queueLimit)
        : this(new DefinedDistribution(delay), name, channelsCount, queueLimit) { }

    public Process(double delay, string name, int queueLimit)
        : this(delay, name, 1, queueLimit) { }

    public Process(string name, int queueLimit)
        : this(1, name, 1, queueLimit) { }

    public string Name { get; } = name;

    private readonly ConcurrentQueue<Request> _requestQueue = [];

    private long _rejectedCount;
    public long RejectedCount => _rejectedCount;

    public DeviceStatistics Statistics { get; } = new(channelsCount);
    public int QueueLength => _requestQueue.Count;

    public void ProcessRequest(Request request)
    {
        if (_requestQueue.Count >= queueLimit)
        {
            Interlocked.Increment(ref _rejectedCount);
            request.Dispose();
            return;
        }

        _requestQueue.Enqueue(request);
    }

    public override async Task RunAsync(CancellationToken ct = default)
    {
        var channelTasks = new Task[channelsCount];
        for (var c = 0; c < channelsCount; c++)
        {
            channelTasks[c] = Task.Run(async () => { await RunSingleAsync(ct); }, ct);
        }

        await Task.WhenAll(channelTasks);
    }

    private async Task RunSingleAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            while (_requestQueue.IsEmpty && !ct.IsCancellationRequested)
            {
                await Task.Delay(50, ct);
            }

            if (ct.IsCancellationRequested) break;

            if (!_requestQueue.TryDequeue(out var request))
            {
                continue;
            }

            var serviceTime = Distribution.Generate();

            Statistics.RecordWork(serviceTime);
            await Task.Delay(TimeSpan.FromSeconds(serviceTime), ct);

            var next = NodeMap.GetNextNode();
            next.ProcessRequest(request);
        }
    }
}