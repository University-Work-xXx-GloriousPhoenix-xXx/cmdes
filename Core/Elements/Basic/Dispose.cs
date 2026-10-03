using Core.Hierarchy;

namespace Core.Elements.Basic;

public class Dispose<TRequest> : Element, IDestinationElement<TRequest>
{
    public bool InAct(TRequest request)
    {
        Quantity++;
        return true;
    }
}