using Lab2DES.Elements;

namespace Lab2DES.Routing;

public abstract class Router<TRequest> : Element, IDestinationElement<TRequest>
{
    public abstract bool InAct(TRequest request);
}