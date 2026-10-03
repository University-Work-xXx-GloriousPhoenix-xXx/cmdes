using Core.Elements.Basic;
using Core.Hierarchy;

namespace Lab3.Bank;

public class BankProcess<TRequest>(Process<TRequest> laneA, Process<TRequest> laneB) : Element, IDestinationElement<TRequest>
{
    public Process<TRequest> LaneA { get; } = laneA;
    public Process<TRequest> LaneB { get; } = laneB;

    public int IncomingAttempts { get; private set; } = 0;
    public int Failure { get; private set; } = 0;
    public int SwitchCount { get; private set; } = 0;

    public double MeanTotalClients { get; private set; } = 0.0;

    public int TotalDepartures => LaneA.Quantity + LaneB.Quantity;
    public int TotalClients => LaneA.CurrChannels + LaneA.CurrQueue + LaneB.CurrChannels + LaneB.CurrQueue;

    public bool InAct(TRequest request)
    {
        IncomingAttempts++;

        if (TotalClients >= 8)
        {
            Failure++;
            return false;
        }

        var loadA = LaneA.CurrChannels + LaneA.CurrQueue;
        var loadB = LaneB.CurrChannels + LaneB.CurrQueue;

        var accepted = loadA <= loadB ? LaneA.InAct(request) : LaneB.InAct(request);
        if (!accepted)
        {
            Failure++;
        }

        return accepted;
    }

    public void Calculate(double delta)
    {
        MeanTotalClients += TotalClients * delta;
        CheckJockeying();
    }

    private void CheckJockeying()
    {
        if (Math.Abs(LaneA.CurrQueue - LaneB.CurrQueue) >= 2)
        {
            SwitchCount++;
        }
    }
}