using Lab2.Distributions;
using Lab2.SimulationUtils;

namespace Lab2.SimulationNodes;

public abstract class ParametrizedNode(IDistributionStrategy distribution) : IMassServiceNetworkNode
{
    protected IDistributionStrategy Distribution = distribution;
    protected NextNodeMap NodeMap = new();

    public virtual void SetDistribution(IDistributionStrategy distribution)
    {
        Distribution = distribution;
    }

    public virtual void AddNextNode(IReceiverNode node)
    {
        NodeMap.AddRoute(node, 1.0);
    }

    public virtual void SetNodeMap(NextNodeMap map)
    {
        if (!map.VerifyMap())
        {
            throw new InvalidOperationException("Sum of probabilities of all routes must be 1.0");
        }

        NodeMap = map;
    }

    public abstract Task RunAsync(CancellationToken ct = default);
}