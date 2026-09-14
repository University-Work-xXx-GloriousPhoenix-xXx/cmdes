namespace Lab2.MSNNodes;

public interface IMassServiceNetworkNode
{
    Task RunAsync(CancellationToken ct = default);
}