namespace Lab2.Distributions;

public class UniformDistribution(double d, double s) : IDistributionStrategy
{
    public double Generate()
    {
        var u = Random.Shared.NextDouble();
        return (d - s) + u * (2 * s);
    }
}