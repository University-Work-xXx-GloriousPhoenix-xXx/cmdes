namespace Lab2.SimulationNodes;

public interface IMassServiceNetworkNode
{
    Task RunAsync(CancellationToken ct = default);
}