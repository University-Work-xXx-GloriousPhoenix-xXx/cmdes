namespace Lab2.SimulationNodes;

public interface IReceiverNode : IMassServiceNetworkNode
{
    void ProcessRequest(Request request);
}