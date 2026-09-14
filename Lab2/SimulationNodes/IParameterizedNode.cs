using Lab2.Distributions;

namespace Lab2.SimulationNodes;

public interface IParameterizedNode : IMassServiceNetworkNode
{
    void SetDistribution(IDistributionStrategy distribution);
    void AddNextNode(IReceiverNode node);
}