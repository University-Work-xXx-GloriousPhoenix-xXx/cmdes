namespace Lab2DES.Distributions;

public class UniformDistribution(double timeMin, double timeMax) : IDistributionStrategy
{
    private static readonly Random _random = new();
    public double Generate()
    {
        var a = 0.0;
        while (a == 0)
        {
            a = _random.NextDouble();
        }

        a = timeMin + a * (timeMax - timeMin);
        return a;
    }
}
