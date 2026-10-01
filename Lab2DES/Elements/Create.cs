namespace Lab2DES.Elements;

public class Create : SourceElement
{
    public Create() : base()
    {
        TNext = 0.0;
    }

    public override void OutAct()
    {
        Quantity++;
        TNext = TCurr + GetDelay();
        RouteMap.TryRoute();
    }
}
