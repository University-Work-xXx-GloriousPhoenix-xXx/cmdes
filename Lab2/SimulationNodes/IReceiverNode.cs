using Lab2.SimulationUtils;

namespace Lab2.SimulationNodes;

public interface IReceiverNode : IMassServiceNetworkNode
{
    void ProcessRequest(Request request);
}