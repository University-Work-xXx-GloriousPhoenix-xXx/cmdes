namespace Lab2DES.Distributions;

public class UniformDistribution(double timeMin, double timeMax) : IDistributionStrategy
{
    private static readonly Random Random = new();
    public double Generate()
    {
        return timeMin + Random.NextDouble() * (timeMax - timeMin);
    }
}
