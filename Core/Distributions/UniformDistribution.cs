namespace Core.Distributions;

public class UniformDistribution(double d, double s) : IDistributionStrategy
{
    private static readonly Random Random = new();
    public double Generate()
    {
        var u = Random.NextDouble();
        return (d - s) + u * (2 * s);
    }
}