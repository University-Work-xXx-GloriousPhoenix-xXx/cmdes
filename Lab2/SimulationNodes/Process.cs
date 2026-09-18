using Lab2.Distributions;
using Lab2.Logging;
using System.Collections.Concurrent;

namespace Lab2.SimulationNodes;

public class Process(IDistributionStrategy distribution, string name, int channelsCount) : IProcessingNode
{
    public Process(double delay, string name, int channelsCount) : this(new DefinedDistribution(delay), name, channelsCount) { }
    public Process(double delay, string name) : this(delay, name, 1) { }
    public Process(string name) : this(1, name) { }

    public string Name { get; } = name;

    private IDistributionStrategy _distribution = distribution;

    private readonly List<IReceiverNode> _nextNodes = [];
    private readonly ConcurrentQueue<Request> _requestQueue = [];

    public DeviceStatistics Statistics { get; } = new(channelsCount);
    public int QueueLength => _requestQueue.Count;

    public void AddNextNode(IReceiverNode node) => _nextNodes.Add(node);

    public void ProcessRequest(Request request) => _requestQueue.Enqueue(request);

    public void SetDistribution(IDistributionStrategy distribution) => _distribution = distribution;

    public async Task RunAsync(CancellationToken ct = default)
    {
        var channelTasks = new Task[channelsCount];
        for (var c = 0; c < channelsCount; c++)
        {
            channelTasks[c] = Task.Run(async () =>
            {
                await RunSingleAsync(ct);
            }, ct);
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

            var serviceTime = _distribution.Generate();

            Statistics.RecordWork(serviceTime);
            await Task.Delay(TimeSpan.FromSeconds(serviceTime), ct);

            foreach (var node in _nextNodes)
            {
                node.ProcessRequest(request);
            }
        }
    }
}