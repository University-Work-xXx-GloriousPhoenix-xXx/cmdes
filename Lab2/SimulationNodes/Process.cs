using Lab2.Distributions;
using Lab2.Logging;
using System.Collections.Concurrent;

namespace Lab2.SimulationNodes;

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
    private static readonly Lock ConsoleLock = new();

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

            SimulationLogger.Log(name, $"Started processing request {request.Id}", ConsoleColor.Yellow);

            var serviceTime = _distribution.Generate();
            await Task.Delay(TimeSpan.FromSeconds(serviceTime), ct);

            SimulationLogger.Log(name, $"Completed processing request {request.Id} in {serviceTime:F2}s", ConsoleColor.Green);

            foreach (var node in _nextNodes)
            {
                node.ProcessRequest(request);
            }
        }
    }
}