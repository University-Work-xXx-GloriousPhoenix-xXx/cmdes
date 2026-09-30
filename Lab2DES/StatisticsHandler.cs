using Lab2DES.Elements;

namespace Lab2DES;

public static class StatisticsHandler
{
    public static void Show(this Model model)
    {
        if (model.Elements == null || !model.Elements.Any()) return;

        var maxNameLength = Math.Max(model.Elements.Max(el => el?.Name?.Length ?? 0), "Name".Length);

        var header = $"{"Name".PadRight(maxNameLength)} | {"Quantity",-10} | {"Mean Length",-13} | {"Failure Prob",-13} | {"Mean Load",-11}";
        Console.WriteLine(header);
        Console.WriteLine(new string('-', header.Length));

        foreach (var el in model.Elements)
        {
            var name = el.Name.PadRight(maxNameLength);
            var quantity = el.Quantity.ToString().PadRight(10);

            if (el is Process p)
            {
                var meanLength = model.TCurr > 0 ? p.MeanQueue / model.TCurr : 0.0;
                var failureProb = (p.Quantity + p.Failure) > 0 ? (double)p.Failure / (p.Quantity + p.Failure) : 0.0;

                var maxPossibleLoadTime = model.TCurr * p.MaxChannels;
                var meanLoad = maxPossibleLoadTime > 0 ? (p.MeanLoadTime / maxPossibleLoadTime) * 100 : 0.0;

                Console.WriteLine($"{name} | {quantity} | {meanLength,-13:F4} | {failureProb,-13:F4} | {meanLoad,-6:F2}%");
            }
            else
            {
                Console.WriteLine($"{name} | {quantity} | {"-",-13} | {"-",-13} | {"-",-11}");
            }
        }
    }
    public static void Calculate(this Process process, double delta)
    {
        process.MeanQueue += process.CurrQueue * delta;
        process.MeanLoadTime += process.CurrChannels * delta;
    }
}
