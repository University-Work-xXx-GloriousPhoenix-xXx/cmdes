namespace Core.Hierarchy;

public abstract class Router<TRequest> : Element, IDestinationElement<TRequest>
{
    public abstract bool InAct(TRequest request);
}