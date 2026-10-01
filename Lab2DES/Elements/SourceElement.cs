using Lab2DES.Distributions;

namespace Lab2DES.Elements;

public abstract class SourceElement : Element
{
    public IDistributionStrategy Distribution { get; set; } = new ExponentialDistribution(1);
    public abstract void OutAct();
    protected double GetDelay() => Distribution.Generate();
    public IDestinationElement? NextElement { get; set; } = null;
}