using Lab2.SimulationNodes;
using System.Diagnostics;
using ProcessNode = Lab2.SimulationNodes.Process;

namespace Lab2.Logging;

public static class HealthLogger
{
    public static async Task RunDashboardAsync(IEnumerable<ProcessNode> processors, Create createNode, CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        var procList = processors.ToList();

        var stopwatch = Stopwatch.StartNew();

        Console.Clear();
        Console.CursorVisible = false;

        try
        {
            while (await timer.WaitForNextTickAsync(ct))
            {
                Console.SetCursorPosition(0, 0);

                var uptime = stopwatch.Elapsed;

                Console.WriteLine("==================================================================================");
                Console.WriteLine("                        SYSTEM HEALTH & METRICS DASHBOARD                         ");
                Console.WriteLine("==================================================================================");
                Console.WriteLine(@$" Status: Running | Uptime: {uptime:hh\:mm\:ss}                                     ");
                Console.WriteLine("----------------------------------------------------------------------------------");
                Console.WriteLine($" {"Node / Channel",-22} | {"Load",-6} | {"Queue",-8} | {"Processed",-10} | {"Rejected",-10}");
                Console.WriteLine("----------------------------------------------------------------------------------");

                foreach (var proc in procList)
                {
                    var (utilization, count, _) = proc.Statistics.GetSnapshot();
                    Console.WriteLine($" {proc.Name,-22} | {utilization * 100,5:F1}% | {proc.QueueLength,8} | {count,10} | {proc.RejectedCount,10}");
                }

                Console.WriteLine("==================================================================================");
                Console.WriteLine(" Press Enter to stop simulation...                                                ");
            }
        }
        finally
        {
            Console.CursorVisible = true;
        }
    }
}