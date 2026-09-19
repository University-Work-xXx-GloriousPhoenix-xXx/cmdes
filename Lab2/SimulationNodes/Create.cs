using Lab2.Distributions;
using Lab2.SimulationUtils;

namespace Lab2.SimulationNodes;

public class Create(IDistributionStrategy distribution) : ParametrizedNode(distribution)
{
    public Create(double delay) : this(new DefinedDistribution(delay)) { }

    public Create() : this(1) { }

    public override async Task RunAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            var delay = Distribution.Generate();
            await Task.Delay(TimeSpan.FromSeconds(delay), ct);
            var request = new Request();

            var next = NodeMap.GetNextNode();
            next.ProcessRequest(request);
        }
    }
}