using System.Collections.Concurrent;
using Lab2.Distributions;

namespace Lab2.MSNNodes;

public class Process(IDistributionStrategy distribution, string name) : IProcessingNode
{
    public Process(double delay, string name) : this(new DefinedDistribution(delay), name)
    {
    }

    public Process(string name) : this(1, name)
    {
    }

    private IDistributionStrategy _distribution = distribution;
    private readonly List<IReceiverNode> _nextNodes = [];
    private readonly ConcurrentQueue<Request> _requestQueue = [];

    public void AddNextNode(IReceiverNode node)
    {
        _nextNodes.Add(node);
    }

    public void ProcessRequest(Request request)
    {
        _requestQueue.Enqueue(request);
    }

    public void SetDistribution(IDistributionStrategy distribution)
    {
        _distribution = distribution;
    }

    public async Task RunAsync(CancellationToken ct = default)
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
            await Task.Delay(TimeSpan.FromSeconds(serviceTime), ct);

            foreach (var node in _nextNodes)
            {
                node.ProcessRequest(request);
            }
        }
    }
}