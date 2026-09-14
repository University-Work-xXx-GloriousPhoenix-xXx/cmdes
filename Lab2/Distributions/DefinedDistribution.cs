namespace Lab2.Distributions;

public class DefinedDistribution(double value) : IDistributionStrategy
{
    public double Generate() => value;
}