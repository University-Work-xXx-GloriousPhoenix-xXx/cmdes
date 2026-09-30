namespace Lab2DES.Elements;

public class Create : Element
{
    public Create() : base()
    {
        TNext = 0.0;
    }

    public override void OutAct()
    {
        base.OutAct();
        TNext = TCurr + GetDelay();
        NextElement?.InAct();
    }
}
