using Lab2.Distributions;
using Lab2.Logging;
using Lab2.SimulationNodes;
using Lab2.SimulationUtils;
using System.Globalization;

namespace Lab2.SimulationHandlers;

public static class SimulationOrchestrator
{
    public static async Task RunAsync(SimulationConfig[] runConfigs, string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        var filePath = Path.Combine(outputDirectory, "simulation.csv");
        InitializeCsvFile(filePath);

        for (var i = 0; i < runConfigs.Length; i++)
        {
            var runNumber = i + 1;
            var result = await RunSingleSimulationAsync(runConfigs[i], runNumber);
            AppendResultToCsv(filePath, result);
        }
    }

    private static async Task<SimulationResult> RunSingleSimulationAsync(SimulationConfig config, int runNumber)
    {
        var (p1Devs, p1Q, p1Time) = config.QueuingSystemsDataList[0];
        var (p2Devs, p2Q, p2Time) = config.QueuingSystemsDataList[1];
        var (p3Devs, p3Q, p3Time) = config.QueuingSystemsDataList[2];

        var create = new Create(new ExponentialDistribution(config.AverageArrivalDelay));
        var process1 = new Process(new ExponentialDistribution(p1Time), "Process 1", p1Devs, p1Q);
        var process2 = new Process(new ExponentialDistribution(p2Time), "Process 2", p2Devs, p2Q);
        var process3 = new Process(new ExponentialDistribution(p3Time), "Process 3", p3Devs, p3Q);
        var dispose = new Dispose(0.1);

        SetupRouting(create, process1, process2, process3, dispose, runNumber);

        var cts = new CancellationTokenSource();

        var metrics = new SimulationMetricsCollector(p1Devs, p2Devs, p3Devs, process1, process2, process3);
        var metricsTask = metrics.StartCollectingAsync(cts.Token);
        var monitorTask = SimulationMonitor.RunAsync(create, dispose, [process1, process2, process3], config, cts);

        var simulationTasks = new[]
        {
            Task.Run(() => create.RunAsync(cts.Token), cts.Token),
            Task.Run(() => process1.RunAsync(cts.Token), cts.Token),
            Task.Run(() => process2.RunAsync(cts.Token), cts.Token),
            Task.Run(() => process3.RunAsync(cts.Token), cts.Token),
            Task.Run(() => dispose.RunAsync(cts.Token), cts.Token),
            monitorTask,
            metricsTask,
            Task.Run(() => HealthLogger.RunDashboardAsync([process1, process2, process3], create, cts.Token), cts.Token)
        };

        try
        {
            await Task.WhenAll(simulationTasks);
        }
        catch (OperationCanceledException) { }

        return BuildResult(runNumber, config, process1, process2, process3, dispose, metrics, p1Devs, p1Q, p1Time, p2Devs, p2Q, p2Time, p3Devs, p3Q, p3Time);
    }

    private static void SetupRouting(Create create, Process p1, Process p2, Process p3, Dispose dispose, int runNumber)
    {
        create.AddNextNode(p1);

        var p1Map = new NextNodeMap();
        var p1Prob = runNumber == 9 ? 0.0 : (runNumber == 10 ? 1.0 : 0.7);
        p1Map.AddRoute(p2, p1Prob);
        p1Map.AddRoute(dispose, 1.0 - p1Prob);
        p1.SetNodeMap(p1Map);

        var p2Map = new NextNodeMap();
        p2Map.AddRoute(p3, 0.7);
        p2Map.AddRoute(dispose, 0.3);
        p2.SetNodeMap(p2Map);

        p3.AddNextNode(dispose);
    }

    private static SimulationResult BuildResult(
        int runNumber, SimulationConfig config,
        Process p1, Process p2, Process p3, Dispose dispose,
        SimulationMetricsCollector metrics,
        int p1Devs, int p1Q, double p1Time,
        int p2Devs, int p2Q, double p2Time,
        int p3Devs, int p3Q, double p3Time)
    {
        var totalIncoming = dispose.TotalProcessed + p1.RejectedCount + p2.RejectedCount + p3.RejectedCount;
        var rejectionProbability = totalIncoming > 0 ? (double)(p1.RejectedCount + p2.RejectedCount + p3.RejectedCount) / totalIncoming : 0;

        return new SimulationResult(
            RunNumber: runNumber,
            AverageArrivalDelay: config.AverageArrivalDelay,
            P1Devices: p1Devs, P1Queue: p1Q, P1Time: p1Time,
            P2Devices: p2Devs, P2Queue: p2Q, P2Time: p2Time,
            P3Devices: p3Devs, P3Queue: p3Q, P3Time: p3Time,
            RouteProbabilityP1ToP2: runNumber == 9 ? 0.0 : (runNumber == 10 ? 1.0 : 0.7),
            TotalIncomingRequests: totalIncoming,
            P1Unprocessed: p1.RejectedCount,
            P2Unprocessed: p2.RejectedCount,
            P3Unprocessed: p3.RejectedCount,
            RejectionProbability: Math.Round(rejectionProbability, 3),
            P1AverageQueueLength: metrics.GetP1AvgQueue(),
            P1AverageBusyDevices: metrics.GetP1AvgBusy(p1Devs),
            P2AverageQueueLength: metrics.GetP2AvgQueue(),
            P2AverageBusyDevices: metrics.GetP2AvgBusy(p2Devs),
            P3AverageQueueLength: metrics.GetP3AvgQueue(),
            P3AverageBusyDevices: metrics.GetP3AvgBusy(p3Devs)
        );
    }

    private static void InitializeCsvFile(string filePath)
    {
        const string headers = "RunNumber,AverageArrivalDelay,P1Devices,P1Queue,P1Time,P2Devices,P2Queue,P2Time,P3Devices,P3Queue,P3Time,RouteProbabilityP1ToP2,TotalIncomingRequests,P1Unprocessed,P2Unprocessed,P3Unprocessed,RejectionProbability,P1AverageQueueLength,P1AverageBusyDevices,P2AverageQueueLength,P2AverageBusyDevices,P3AverageQueueLength,P3AverageBusyDevices";
        File.WriteAllText(filePath, headers + "\n");
    }

    private static void AppendResultToCsv(string filePath, SimulationResult r)
    {
        var culture = CultureInfo.InvariantCulture;
        var line = string.Join(",", [
            r.RunNumber.ToString(culture), r.AverageArrivalDelay.ToString(culture),
            r.P1Devices.ToString(culture), r.P1Queue.ToString(culture), r.P1Time.ToString(culture),
            r.P2Devices.ToString(culture), r.P2Queue.ToString(culture), r.P2Time.ToString(culture),
            r.P3Devices.ToString(culture), r.P3Queue.ToString(culture), r.P3Time.ToString(culture),
            r.RouteProbabilityP1ToP2.ToString(culture), r.TotalIncomingRequests.ToString(culture),
            r.P1Unprocessed.ToString(culture), r.P2Unprocessed.ToString(culture), r.P3Unprocessed.ToString(culture),
            r.RejectionProbability.ToString(culture),
            r.P1AverageQueueLength.ToString(culture), r.P1AverageBusyDevices.ToString(culture),
            r.P2AverageQueueLength.ToString(culture), r.P2AverageBusyDevices.ToString(culture),
            r.P3AverageQueueLength.ToString(culture), r.P3AverageBusyDevices.ToString(culture)
        ]);

        File.AppendAllText(filePath, line + "\n");
    }
}