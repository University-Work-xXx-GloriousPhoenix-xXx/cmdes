using Lab2DES.Distributions;
using Lab2DES.Routing;

namespace Lab2DES.Elements;

public abstract class SourceElement : Element, ISourceElement
{
    public IDistributionStrategy Distribution { get; set; } = new ExponentialDistribution(1);
    public RouteMap RouteMap { get; set; } = new();
    public int Failure { get; protected set; } = 0;
    public abstract void OutAct();
    protected double GetDelay() => Distribution.Generate();

    public SourceElement AddRoute(Route route)
    {
        RouteMap.AddRoute(route);
        return this;
    }
    public SourceElement AddRoute(IDestinationElement destination, RoutePriority priority, double probability) =>
        AddRoute(new Route(destination, priority, probability));
    public SourceElement SetRoutes(IDestinationElement destination)
    {
        RouteMap.SetRoutes(destination);
        return this;
    }
    public SourceElement SetRoutes(IEnumerable<Route> routes)
    {
        RouteMap.SetRoutes(routes);
        return this;
    }
    public SourceElement SetRoutes(Route route)
    {
        RouteMap.SetRoutes(route);
        return this;
    }
    public SourceElement SetRoutes(IDestinationElement destination, RoutePriority priority, double probability)
        => SetRoutes(new Route(destination, priority, probability));

}