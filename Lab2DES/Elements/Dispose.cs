namespace Lab2DES.Elements;

public class Dispose<TRequest> : Element, IDestinationElement<TRequest>
{
    public bool InAct(TRequest request)
    {
        Quantity++;
        return true;
    }
}