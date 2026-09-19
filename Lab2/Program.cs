using Lab2.Distributions;
using Lab2.Logging;
using Lab2.SimulationNodes;
using Lab2.SimulationUtils;

// Nodes declaration
var create = new Create(0.2);
var process1 = new Process(new ExponentialDistribution(1.2), "Process 1", 5, 10);
var process2 = new Process(new ExponentialDistribution(2), "Process 2", 7, 8);
var process3 = new Process(new ExponentialDistribution(1), "Process 3", 2, 1);
var dispose = new Dispose(0.1);

// Connect nodes
create.AddNextNode(process1);

var process1Map = new NextNodeMap();
process1Map.AddRoute(process2, 0.7);
process1Map.AddRoute(dispose, 0.3);
process1.SetNodeMap(process1Map);

var process2Map = new NextNodeMap();
process2Map.AddRoute(process3, 0.7);
process2Map.AddRoute(dispose, 0.3);
process2.SetNodeMap(process2Map);

process3.AddNextNode(dispose);

// Run the simulation
var cts = new CancellationTokenSource();
var simulationTasks = new[]
{
    Task.Run(() => create.RunAsync(cts.Token), cts.Token),
    Task.Run(() => process1.RunAsync(cts.Token), cts.Token),
    Task.Run(() => process2.RunAsync(cts.Token), cts.Token),
    Task.Run(() => process3.RunAsync(cts.Token), cts.Token),
    Task.Run(() => dispose.RunAsync(cts.Token), cts.Token),

    Task.Run(() => HealthLogger.RunDashboardAsync([process1, process2, process3], create, cts.Token)), 
};

Console.WriteLine("Simulation is running. Press Enter to stop...");
Console.ReadLine();

await cts.CancelAsync();

try
{
    await Task.WhenAll(simulationTasks);
}
catch (OperationCanceledException)
{
}

Console.WriteLine("Simulation successfully stopped.");

