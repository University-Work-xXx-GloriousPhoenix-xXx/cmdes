using Lab2.Distributions;
using Lab2.Logging;

namespace Lab2.SimulationNodes;

public class Create(IDistributionStrategy distribution) : IGeneratorNode
{
    public Create(double delay) : this(new DefinedDistribution(delay))
    {
    }

    public Create() : this(1)
    {
    }

    private IDistributionStrategy _distribution = distribution;
    private readonly List<IReceiverNode> _nextNodes = [];

    public void AddNextNode(IReceiverNode node)
    {
        _nextNodes.Add(node);
    }

    public void SetDistribution(IDistributionStrategy distribution)
    {
        _distribution = distribution;
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            var delay = _distribution.Generate();
            await Task.Delay(TimeSpan.FromSeconds(delay), ct);
            var request = new Request();

            SimulationLogger.Log("CREATE", $"Created request {request.Id}", ConsoleColor.Cyan);

            foreach (var node in _nextNodes)
            {
                node.ProcessRequest(request);
            }
        }
    }
}