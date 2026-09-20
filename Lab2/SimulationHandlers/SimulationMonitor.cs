using Lab2.SimulationNodes;
using Lab2.SimulationUtils;

namespace Lab2.SimulationHandlers;

public static class SimulationMonitor
{
    public static async Task RunAsync(Create create, Dispose dispose, Process[] processes, SimulationConfig config, CancellationTokenSource cts)
    {
        while (!cts.Token.IsCancellationRequested)
        {
            if (create.GeneratedCount >= config.RequestCount)
            {
                var totalFinalized = dispose.TotalProcessed + processes.Sum(p => p.RejectedCount);

                if (totalFinalized >= config.RequestCount)
                {
                    await Task.Delay(50, cts.Token);
                    await cts.CancelAsync();
                    break;
                }
            }
            await Task.Delay(50, cts.Token);
        }
    }
}