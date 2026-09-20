namespace Lab2.Distributions;

public class NormalDistribution(double mean, double stdDev) : IDistributionStrategy
{
    public double Generate()
    {
        var u1 = 1.0 - Random.Shared.NextDouble();
        var u2 = 1.0 - Random.Shared.NextDouble();
        var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

        return mean + stdDev * randStdNormal;
    }
}