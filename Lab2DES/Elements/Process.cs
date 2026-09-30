namespace Lab2DES.Elements;

public class Process() : Element()
{
    public int QueueLength { get; private set; } = 0;
    public int MaxQueue { get; set; } = int.MaxValue;
    public int Failure { get; set; } = 0;
    public double MeanQueue { get; set; } = 0.0;
    public int State { get; private set; } = 0;

    public override void InAct()
    {
        if (State == 0)
        {
            State = 1;
            TNext = TCurr + GetDelay();
        }
        else
        {
            if (QueueLength < MaxQueue)
            {
                QueueLength++;
            }
            else
            {
                Failure++;
            }
        }
    }

    public override void OutAct()
    {
        base.OutAct();
        TNext = double.MaxValue;
        State = 0;

        if (QueueLength > 0)
        {
            QueueLength--;
            State = 1;
            TNext = TCurr + GetDelay();
        }

        NextElement?.InAct();
    }
}
