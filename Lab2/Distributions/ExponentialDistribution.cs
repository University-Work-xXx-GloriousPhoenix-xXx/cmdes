namespace Lab2DES.Distributions;

public class ExponentialDistribution(double timeMean) : IDistributionStrategy
{
    private static readonly Random _random = new();
    public double Generate()
    {
        var a = 0.0;
        while (a == 0)
        {
            a = _random.NextDouble();
        }

        a = -timeMean * Math.Log(a);
        return a;
    }
}
