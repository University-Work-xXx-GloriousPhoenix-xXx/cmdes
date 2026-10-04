using Core.Elements.Basic;
using Core.Hierarchy;

namespace Core;

public class Model<TRequest>(IList<Element> elements)
{
    public IList<Element> Elements { get; init; } = elements;
    public double TCurr { get; private set; } = 0.0;
    public long TotalEventsCount { get; private set; } = 0;
    public long? MaxEventsLimit { get; set; }
    private const double Epsilon = 1e-9;
    public Action<double>? OnCustomStep { get; set; }

    public Model<TRequest> Simulate(double time)
    {
        while (TCurr < time && (!MaxEventsLimit.HasValue || TotalEventsCount < MaxEventsLimit.Value))
        {
            SimulateStep(time);
        }

        return this;
    }

    private void SimulateStep(double time)
    {
        var tNext = Elements.Min(el => el.TNext);

        if (tNext > time)
        {
            var finalDelta = time - TCurr;
            if (finalDelta > 0)
            {
                ExecuteStep(finalDelta);
            }
            TCurr = time;
            return;
        }

        var delta = tNext - TCurr;
        if (delta > 0)
        {
            ExecuteStep(delta);
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

            if (el is SourceElement<TRequest> sourceEl)
            {
                sourceEl.OutAct();
            }
            else if (el is Process<TRequest> proc)
            {
                proc.OutAct();
            }
        }

        TotalEventsCount++;
    }

    private void ExecuteStep(double delta)
    {
        foreach (var p in Elements.OfType<Process<TRequest>>())
        {
            p.MeanQueue += p.CurrQueue * delta;
            p.MeanLoadTime += p.CurrChannels * delta;
        }

        OnCustomStep?.Invoke(delta);
    }
}