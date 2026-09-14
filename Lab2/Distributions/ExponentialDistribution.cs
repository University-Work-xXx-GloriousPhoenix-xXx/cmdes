namespace Lab2.Distributions;

public class ExponentialDistribution(double d) : IDistributionStrategy
{
    public double Generate()
    {
        var u = Random.Shared.NextDouble();
        u = u == 0 ? 1e-10 : u;
        return -Math.Log(u) / d;
    }
}