namespace Lab2DES.Elements;

public class Process<TRequest> : SourceElement<TRequest>, IDestinationElement<TRequest>
{
    public int MaxChannels { get; set; } = 1;
    public int CurrChannels => _channelQueue.Count;
    private readonly PriorityQueue<TRequest, double> _channelQueue = new();

    public int MaxQueue { get; set; } = int.MaxValue;
    private readonly Queue<TRequest> _waitingQueue = new();
    public int CurrQueue => _waitingQueue.Count;

    public Func<TRequest, double>? ServiceTimeCalculator { get; set; }

    public double MeanQueue { get; set; } = 0.0;
    public double MeanLoadTime { get; set; } = 0.0;
    public int Failure { get; private set; } = 0;
    public int IncomingAttempts { get; private set; } = 0;


    public bool InAct(TRequest request)
    {
        IncomingAttempts++;

        if (CurrChannels < MaxChannels)
        {
            var serviceTime = ServiceTimeCalculator?.Invoke(request) ?? GetDelay();

            var departureTime = TCurr + serviceTime;
            _channelQueue.Enqueue(request, departureTime);
            UpdateTNext();
            return true;
        }

        if (CurrQueue < MaxQueue)
        {
            _waitingQueue.Enqueue(request);
            return true;
        }

        Failure++;
        return false;
    }

    public override void OutAct()
    {
        Quantity++;
        TRequest? departingRequest = default;
        if (_channelQueue.TryDequeue(out var req, out _))
        {
            departingRequest = req;
        }

        if (_waitingQueue.Count > 0)
        {
            var nextRequest = _waitingQueue.Dequeue();
            var serviceTime = ServiceTimeCalculator?.Invoke(nextRequest) ?? GetDelay();

            var departureTime = TCurr + serviceTime;
            _channelQueue.Enqueue(nextRequest, departureTime);
        }

        UpdateTNext();

        if (departingRequest != null)
        {
            NextElement?.InAct(departingRequest);
        }
    }

    private void UpdateTNext()
    {
        TNext = _channelQueue.TryPeek(out _, out var nextTime)
            ? nextTime
            : double.MaxValue;
    }

    public void ForceInitialize(int initialQueueCount, TRequest initialRequest, double initialDepartureTime)
    {
        _waitingQueue.Clear();
        _channelQueue.Clear();

        for (var i = 0; i < initialQueueCount; i++)
        {
            _waitingQueue.Enqueue(initialRequest);
        }

        _channelQueue.Enqueue(initialRequest, initialDepartureTime);
        UpdateTNext();
    }
}
