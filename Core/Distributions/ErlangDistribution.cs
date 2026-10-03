namespace Core.Distributions;

public class ErlangDistribution(int k, double timeMean) : IDistributionStrategy
{
    private static readonly Random Random = new();
    private readonly int _k = k > 0 ? k : throw new ArgumentException("Shape parameter k must be greater than 0.", nameof(k));

    public double Generate()
    {
        var sum = 0.0;
        var beta = timeMean / _k;

        for (var i = 0; i < _k; i++)
        {
            var a = 0.0;
            while (a == 0)
            {
                a = Random.NextDouble();
            }

            sum += -beta * Math.Log(a);
        }

        return sum;
    }
}