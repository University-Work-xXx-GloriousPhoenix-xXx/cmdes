using Lab2DES.Elements;

namespace Lab2DES.Routing;

public abstract class Router : IDestinationElement
{
    public abstract bool InAct();
}