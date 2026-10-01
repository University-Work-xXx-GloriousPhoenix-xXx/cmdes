using Lab2DES.Elements;

namespace Lab3;

public class BankProcess(Process laneA, Process laneB) : Element, IDestinationElement
{
    public Process LaneA { get; } = laneA;
    public Process LaneB { get; } = laneB;

    public int IncomingAttempts { get; private set; } = 0;
    public int Failure { get; private set; } = 0;
    public int SwitchCount { get; private set; } = 0;

    public double MeanTotalClients { get; private set; } = 0.0;

    public int TotalDepartures => LaneA.Quantity + LaneB.Quantity;
    public int TotalClients => LaneA.CurrChannels + LaneA.CurrQueue + LaneB.CurrChannels + LaneB.CurrQueue;

    public bool InAct()
    {
        IncomingAttempts++;

        if (TotalClients >= 8)
        {
            Failure++;
            return false;
        }

        int loadA = LaneA.CurrChannels + LaneA.CurrQueue;
        int loadB = LaneB.CurrChannels + LaneB.CurrQueue;

        bool accepted;
        if (loadA <= loadB)
        {
            accepted = LaneA.InAct();
        }
        else
        {
            accepted = LaneB.InAct();
        }

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
        if (LaneA.CurrQueue - LaneB.CurrQueue >= 2)
        {
            SwitchCount++;
        }
        else if (LaneB.CurrQueue - LaneA.CurrQueue >= 2)
        {
            SwitchCount++;
        }
    }
}