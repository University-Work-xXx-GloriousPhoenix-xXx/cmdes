namespace Lab3.Bank;

public static class BankStatisticsHandler<TRequest>
{
    public static void ShowBankReport(BankModel<TRequest> model, BankProcess<TRequest> bank)
    {
        var tCurr = model.TCurr;
        if (tCurr <= 0) return;

        var loadA = (bank.LaneA.MeanLoadTime / (tCurr * bank.LaneA.MaxChannels)) * 100;
        var loadB = (bank.LaneB.MeanLoadTime / (tCurr * bank.LaneB.MaxChannels)) * 100;
        Console.WriteLine($"1. Average Lane A Load:           {loadA,6:F2}%");
        Console.WriteLine($"   Average Lane B Load:           {loadB,6:F2}%");

        var meanClientsInBank = bank.MeanTotalClients / tCurr;
        Console.WriteLine($"2. Mean Clients in Bank:           {meanClientsInBank:F4}");

        var totalDepartures = bank.TotalDepartures;
        var meanDepartureInterval = totalDepartures > 0 ? tCurr / totalDepartures : 0.0;
        Console.WriteLine($"3. Mean Departure Interval:        {meanDepartureInterval:F4} time units");

        var lambda = totalDepartures > 0 ? totalDepartures / tCurr : 0.0;
        var meanStayTime = lambda > 0 ? meanClientsInBank / lambda : 0.0;
        Console.WriteLine($"4. Mean Time in Bank:              {meanStayTime:F4} time units");

        var meanQueueA = bank.LaneA.MeanQueue / tCurr;
        var meanQueueB = bank.LaneB.MeanQueue / tCurr;
        Console.WriteLine($"5. Mean Lane A Queue Length:       {meanQueueA:F4}");
        Console.WriteLine($"   Mean Lane B Queue Length:       {meanQueueB:F4}");

        var failureProb = bank.IncomingAttempts > 0 ? ((double)bank.Failure / bank.IncomingAttempts) * 100 : 0.0;
        Console.WriteLine($"6. Rejection Rate:               {failureProb,6:F2}% (Attempts: {bank.IncomingAttempts}, Failures: {bank.Failure})");

        Console.WriteLine($"7. Lane Switches (Jockeying):      {bank.SwitchCount}");
    }
}