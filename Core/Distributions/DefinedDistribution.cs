namespace Core.Distributions;

public class DefinedDistribution(double value) : IDistributionStrategy
{
    public double Generate() => value;
}