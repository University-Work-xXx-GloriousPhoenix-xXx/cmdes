using Lab2.SimulationNodes;

namespace Lab2.Logging;

public static class LoadLogger
{
    public static async Task StartPeriodicReporter(IEnumerable<Process> processors, TimeSpan interval, CancellationToken ct)
    {
        var procList = processors.ToList();

        using var timer = new PeriodicTimer(interval);

        try
        {
            while (await timer.WaitForNextTickAsync(ct))
            {
                SimulationLogger.Log("MONITOR", "--------------------------------------- Periodic Report ----------------------------------------", ConsoleColor.White);

                foreach (var proc in procList)
                {
                    var (utilization, count, uptime) = proc.Statistics.GetSnapshot();
                    SimulationLogger.Log("MONITOR", $"{proc.Name} | Utilization: {utilization * 100:F2}% | Processed: {count} requests | Uptime: {uptime:F1}s", ConsoleColor.White);
                }

                SimulationLogger.Log("MONITOR", "------------------------------------------------------------------------------------------------", ConsoleColor.White);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
