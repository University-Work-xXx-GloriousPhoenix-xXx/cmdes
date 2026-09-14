namespace Lab2.MSNNodes;

public interface IReceiverNode : IMassServiceNetworkNode
{
    void ProcessRequest(Request request);
}