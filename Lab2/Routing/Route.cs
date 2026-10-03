using Core.Hierarchy;

namespace Lab2.Routing;

public record Route<TRequest>
{
    public IDestinationElement<TRequest> Destination { get; init; }
    public RoutePriority Priority { get; init; }
    public double Probability { get; init; }

    public Route(IDestinationElement<TRequest> destination, RoutePriority priority, double probability)
    {
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(probability);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(probability, 1.0);

        Destination = destination;
        Priority = priority;
        Probability = probability;
    }
}
