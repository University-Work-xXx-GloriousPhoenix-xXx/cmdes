using Lab2.Logging;
using Lab2.SimulationNodes;

// Nodes declaration
var create = new Create(0.5);
var process1 = new Process(4, "Process 1", 8);
var process2 = new Process(3, "Process 2", 6);
var process3 = new Process(2, "Process 3",4);
var dispose = new Dispose(1);

// Connect nodes
create.AddNextNode(process1);
process1.AddNextNode(process2);
process2.AddNextNode(process3);
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

