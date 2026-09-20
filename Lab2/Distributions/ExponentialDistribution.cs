namespace Lab2.Distributions;

public class ExponentialDistribution(double mean) : IDistributionStrategy
{
    public double Generate()
    {
        var u = Random.Shared.NextDouble();
        u = u == 0 ? 1e-10 : u;
        return -Math.Log(u) * mean;
    }
}