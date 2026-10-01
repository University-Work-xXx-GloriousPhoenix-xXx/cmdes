namespace Lab2DES.Elements;

public class Dispose() : Element(), IDestinationElement
{
    public bool InAct()
    {
        Quantity++;
        return true;
    }
}