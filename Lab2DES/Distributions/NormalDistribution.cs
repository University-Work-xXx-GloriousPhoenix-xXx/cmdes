namespace Lab2DES.Distributions;

public class NormalDistribution(double timeMean, double timeDeviation) : IDistributionStrategy
{
    private static readonly Random _random = new();
    public double Generate()
    {
        var u1 = 1.0 - _random.NextDouble();
        var u2 = 1.0 - _random.NextDouble();
        var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return timeMean + timeDeviation * randStdNormal;
    }
}
