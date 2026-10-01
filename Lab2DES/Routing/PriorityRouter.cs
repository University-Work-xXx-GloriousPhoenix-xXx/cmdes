using Lab2DES.Elements;

namespace Lab2DES.Routing;

public class PriorityRouter : Router
{
    public PriorityRouter() { }
    public PriorityRouter(IEnumerable<Route> routes) => SetRoutes(routes);
    public List<Route> Routes { get; private set; } = [];

    public PriorityRouter AddRoute(Route route)
    {
        var groupSum = Routes.Where(r => r.Priority == route.Priority).Sum(r => r.Probability);
        if (groupSum + route.Probability - 1 > 1e-9)
        {
            throw new ArgumentException($"Total probability for priority '{route.Priority}' cannot exceed 1.0");
        }

        Routes.Add(route);
        return this;
    }

    public PriorityRouter AddRoute(IDestinationElement destination, RoutePriority priority, double probability) =>
        AddRoute(new Route(destination, priority, probability));

    public PriorityRouter SetRoutes(IEnumerable<Route> routes)
    {
        var list = routes.ToList();
        foreach (var group in list.GroupBy(r => r.Priority))
        {
            var sum = group.Sum(r => r.Probability);
            if (Math.Abs(sum - 1) > 1e-9)
                throw new ArgumentException($"Total probability for priority '{group.Key}' cannot exceed 1.0");
        }

        Routes = list;
        return this;
    }

    public PriorityRouter SetRoutes(Route route)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(route.Probability, 1);
        Routes = [route];
        return this;
    }

    public PriorityRouter SetRoutes(IDestinationElement destination, RoutePriority priority, double probability) =>
        SetRoutes(new Route(destination, priority, probability));

    public PriorityRouter SetRoutes(IDestinationElement destination) =>
        SetRoutes(destination, RoutePriority.Medium, 1);

    public override bool InAct()
    {
        if (Routes.Count == 0) return false;

        return         Routes
            .GroupBy(r => r.Priority)
            .OrderByDescending(g => g.Key)
            .SelectMany(group => group)
            .Any(route => route.Destination.InAct());
    }
}