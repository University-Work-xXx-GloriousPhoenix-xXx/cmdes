using Lab2DES.Elements;
namespace Lab2DES;

public class Model<TRequest>(IList<Element> elements)
{
    public IList<Element> Elements { get; init; } = elements;
    public double TNext { get; private set; } = 0.0;
    public double TCurr { get; private set; } = 0.0;
    private const double Epsilon = 1e-9;

    public Model<TRequest> Simulate(double time)
    {
        while (TCurr < time)
        {
            TNext = Elements.Min(el => el.TNext);
            var delta = TNext - TCurr;
            foreach (var p in Elements.OfType<Process<TRequest>>())
            {
                StatisticsHandler<TRequest>.Calculate(p, delta);
            }

            TCurr = TNext;
            foreach (var element in Elements)
            {
                element.TCurr = TCurr;
            }

            foreach (var el in Elements.OfType<SourceElement<TRequest>>())
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