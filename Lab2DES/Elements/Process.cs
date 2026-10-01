namespace Lab2DES.Elements;

public class Process() : SourceElement, IDestinationElement
{
    public int MaxChannels { get; set; } = 1;
    public int CurrChannels => _channelQueue.Count;
    private readonly PriorityQueue<double, double> _channelQueue = new();

    public int MaxQueue { get; set; } = 0;
    public int CurrQueue { get; private set; } = 0;
    public double MeanQueue { get; set; } = 0.0;
    public double MeanLoadTime { get; set; } = 0.0;
    public int Failure { get; private set; } = 0;
    public int IncomingAttempts { get; private set; } = 0;


    public bool InAct()
    {
        IncomingAttempts++;

        if (CurrChannels < MaxChannels)
        {
            var departureTime = TCurr + GetDelay();
            _channelQueue.Enqueue(departureTime, departureTime);
            UpdateTNext();
            return true;
        }
        else if (CurrQueue < MaxQueue)
        {
            CurrQueue++;
            return true;
        }
        else
        {
            Failure++;
            return false;
        }
    }

    public override void OutAct()
    {
        Quantity++;
        if (CurrChannels > 0)
        {
            _channelQueue.Dequeue();
        }

        if (CurrQueue > 0)
        {
            CurrQueue--;
            var departureTime = TCurr + GetDelay();
            _channelQueue.Enqueue(departureTime, departureTime);
        }

        UpdateTNext();

        NextElement?.InAct();
    }

    private void UpdateTNext()
    {
TNext = _channelQueue.TryPeek(out var nextTime, out _)
    ? nextTime
    : double.MaxValue;
    }
}
