using Lab2.SimulationNodes;

namespace Lab2.SimulationUtils;

public class NextNodeMap
{
    private readonly List<(IReceiverNode Node, double Probability)> _routes = [];
    public void AddRoute(IReceiverNode node, double probability)
    {
        _routes.Add((node, probability));
    }

    public bool VerifyMap()
    {
        var cumulative = _routes.Sum(r => r.Probability);
        return Math.Abs(cumulative - 1.0) <= 1e-6;
    }

    public IReceiverNode GetNextNode()
    {
        var roll = Random.Shared.NextDouble();
        var cumulative = 0.0;

        foreach (var (node, probability) in _routes)
        {
            cumulative += probability;
            if (roll <= cumulative)
            {
                return node;
            }
        }

        return _routes[0].Node;
    }
}
