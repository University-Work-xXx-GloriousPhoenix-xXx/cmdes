using Core.Distributions;

namespace Core.Hierarchy;
public abstract class SourceElement<TRequest> : Element
{
    public IDistributionStrategy Distribution { get; set; } = new ExponentialDistribution(1);
    public abstract void OutAct();
    protected double GetDelay() => Distribution.Generate();
    public IDestinationElement<TRequest>? NextElement { get; set; } = null;
}