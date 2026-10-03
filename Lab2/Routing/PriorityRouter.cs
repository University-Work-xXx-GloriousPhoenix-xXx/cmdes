
using Core.Hierarchy;

namespace Lab2.Routing;

public class PriorityRouter<TRequest> : Router<TRequest>
{
    public PriorityRouter() { }
    public PriorityRouter(IEnumerable<Route<TRequest>> routes) => SetRoutes(routes);
    public List<Route<TRequest>> Routes { get; private set; } = [];

    public PriorityRouter<TRequest> AddRoute(Route<TRequest> route)
    {
        var groupSum = Routes.Where(r => r.Priority == route.Priority).Sum(r => r.Probability);
        if (groupSum + route.Probability - 1 > 1e-9)
        {
            throw new ArgumentException($"Total probability for priority '{route.Priority}' cannot exceed 1.0");
        }

        Routes.Add(route);
        return this;
    }

    public PriorityRouter<TRequest> AddRoute(IDestinationElement<TRequest> destination, RoutePriority priority, double probability) =>
        AddRoute(new Route<TRequest>(destination, priority, probability));

    public PriorityRouter<TRequest> SetRoutes(IEnumerable<Route<TRequest>> routes)
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

    public PriorityRouter<TRequest> SetRoutes(Route<TRequest> route)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(route.Probability, 1);
        Routes = [route];
        return this;
    }

    public PriorityRouter<TRequest> SetRoutes(IDestinationElement<TRequest> destination, RoutePriority priority, double probability) =>
        SetRoutes(new Route<TRequest>(destination, priority, probability));

    public PriorityRouter<TRequest> SetRoutes(IDestinationElement<TRequest> destination) =>
        SetRoutes(destination, RoutePriority.Medium, 1);

    public override bool InAct(TRequest request)
    {
        if (Routes.Count == 0) return false;

        return         Routes
            .GroupBy(r => r.Priority)
            .OrderByDescending(g => g.Key)
            .SelectMany(group => group)
            .Any(route => route.Destination.InAct(request));
    }
}