using Lab2DES.Distributions;

namespace Lab2DES.Elements;

public abstract class Element
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = nameof(Element);
    public double TCurr { get; set; } = 0.0;
    public double TNext { get; protected set; } = double.MaxValue;
    public int Quantity { get; protected set; } = 0;
    public IDistributionStrategy Distribution { get; set; } = new ExponentialDistribution(1);
    public Element? NextElement { get; set; } = null;

    public virtual double GetDelay() => Distribution.Generate();

    public virtual void InAct() => NextElement?.InAct();
    public virtual void OutAct() => Quantity++;
}