namespace Core.Hierarchy;

public abstract class Element
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = nameof(Element);
    public double TCurr { get; set; } = 0.0;
    public double TNext { get; set; } = double.MaxValue;
    public int Quantity { get; protected set; } = 0;
}
