using Lab2DES.Elements;

namespace Lab2DES.Routing;

public record Route
{
    public IDestinationElement Destination { get; init; }
    public RoutePriority Priority { get; init; }
    public double Probability { get; init; }

    public Route(IDestinationElement destination, RoutePriority priority, double probability)
    {
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(probability);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(probability, 1.0);

        Destination = destination;
        Priority = priority;
        Probability = probability;
    }
}
