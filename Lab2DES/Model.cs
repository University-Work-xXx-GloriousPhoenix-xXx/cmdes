using Lab2DES.Elements;
namespace Lab2DES;

public class Model(IList<Element> elements)
{
    public IList<Element> Elements { get; init; } = elements;
    public double TNext { get; private set; } = 0.0;
    public double TCurr { get; private set; } = 0.0;
    private const double Epsilon = 1e-9;

    public Model Simulate(double time)
    {
        while (TCurr < time)
        {
            TNext = Elements.Min(el => el.TNext);
            foreach (var p in Elements.OfType<Process>())
            {
                p.Calculate(TNext - TCurr);
            }

            TCurr = TNext;
            foreach (var element in Elements)
            {
                element.TCurr = TCurr;
            }

            foreach (var el in Elements)
            {
                if (Math.Abs(el.TNext - TNext) < Epsilon)
                {
                    el.OutAct();
                }
            }
        }

        return this;
    }
}