using Lab2DES;
using Lab2DES.Elements;

namespace Lab3;

public class BankModel(IList<Element> elements, BankProcess bank)
{
    public IList<Element> Elements { get; init; } = elements;
    public BankProcess Bank { get; init; } = bank;
    public double TCurr { get; private set; } = 0.0;
    private const double Epsilon = 1e-9;

    public BankModel Simulate(double time)
    {
        while (TCurr < time)
        {
            var tNext = Elements.Min(el => el.TNext);

            if (tNext > time)
            {
                var finalDelta = time - TCurr;
                if (finalDelta > 0)
                {
                    Bank.Calculate(finalDelta);
                    foreach (var p in Elements.OfType<Process>()) p.Calculate(finalDelta);
                }
                TCurr = time;
                break;
            }

            var delta = tNext - TCurr;

            if (delta > 0)
            {
                Bank.Calculate(delta);
                foreach (var p in Elements.OfType<Process>())
                {
                    p.Calculate(delta);
                }
            }

            TCurr = tNext;
            foreach (var element in Elements)
            {
                element.TCurr = TCurr;
            }

            foreach (var el in Elements)
            {
                if (Math.Abs(el.TNext - TCurr) > Epsilon)
                    continue;

                if (el is SourceElement sourceEl)
                {
                    sourceEl.OutAct();
                }
                else if (el is Process proc)
                {
                    proc.OutAct();
                }
            }
        }

        return this;
    }
}