using Lab2.Distributions;
using Lab2.SimulationUtils;

namespace Lab2.SimulationNodes;

public class Create(IDistributionStrategy distribution, int requestCount) : ParametrizedNode(distribution)
{
    public Create(IDistributionStrategy distribution) : this(distribution, int.MaxValue) { }
    public Create(double delay, int requestCount) : this(new ExponentialDistribution(delay), requestCount) { }
    public Create(double delay) : this(delay, int.MaxValue) { }
    public Create() : this(1.0) { }

    public int GeneratedCount { get; private set; }

    public override async Task RunAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested && GeneratedCount < requestCount)
        {
            var delay = Distribution.Generate();
            await Task.Delay(TimeSpan.FromSeconds(delay), ct);
            var request = new Request();
            GeneratedCount++;

            var next = NodeMap.GetNextNode();
            next.ProcessRequest(request);
        }
    }
}