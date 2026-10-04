using System.Diagnostics;
using Core;
using Core.Distributions;
using Core.Elements.Basic;
using Core.Hierarchy;
using Lab2.Routing;

namespace Lab4;

public class ComplexityTester(IEnumerable<int> nodeCounts, IEnumerable<int> eventCounts)
{
    private static readonly IDistributionStrategy CreationDistribution = new ExponentialDistribution(2);
    private static readonly IDistributionStrategy ProcessDistribution = new ExponentialDistribution(1);
    public void RunSimple(double simulationTime)
    {
        Console.WriteLine("=== LINEAR NETWORK BENCHMARK ===");
        Console.WriteLine($"{"N",-8} | {"E",-10} | {"T (ms)",-10}");
        Console.WriteLine(new string('-', 35));

        foreach (var n in nodeCounts)
        {
            foreach (var e in eventCounts)
            {
                RunSimpleOnce(simulationTime, n, e);
            }
        }
    }

    public void RunBranched(double simulationTime, int k)
    {
        Console.WriteLine("\n=== BRANCHED NETWORK BENCHMARK ===");
        Console.WriteLine($"{"N",-8} | {"E",-10} | {"T (ms)",-10}");
        Console.WriteLine(new string('-', 35));

        foreach (var n in nodeCounts)
        {
            foreach (var e in eventCounts)
            {
                RunBranchedOnce(simulationTime, n, e, k);
            }
        }
    }
    public void RunBranched(double simulationTime)
    {
        Console.WriteLine("\n=== BRANCHED NETWORK BENCHMARK ===");
        Console.WriteLine($"{"N",-8} | {"E",-10} | {"T (ms)",-10}");
        Console.WriteLine(new string('-', 35));

        foreach (var n in nodeCounts)
        {
            foreach (var e in eventCounts)
            {
                RunBranchedOnce(simulationTime, n, e, n);
            }
        }
    }

    private static void RunSimpleOnce(double simulationTime, int n, int e)
    {
        var c = new Create<int>
        {
            Name = "Create",
            Distribution = CreationDistribution,
            RequestFactory = () => Random.Shared.Next()
        };

        List<Element> list = [c];
        SourceElement<int> element = c;

        for (var i = 0; i < n; i++)
        {
            var next = new Process<int>
            {
                Name = $"Process {i + 1}",
                Distribution = ProcessDistribution,
            };
            element.NextElement = next;
            list.Add(next);
            element = next;
        }

        var d = new Dispose<int> { Name = "Dispose" };
        element.NextElement = d;
        list.Add(d);

        var model = new Model<int>(list)
        {
            MaxEventsLimit = e
        };

        var sw = Stopwatch.StartNew();
        model.Simulate(simulationTime);
        sw.Stop();

        Console.WriteLine($"{n,-8} | {model.TotalEventsCount,-10} | {sw.ElapsedMilliseconds,-10}");
    }

    private static void RunBranchedOnce(double simulationTime, int n, int e, int k)
    {
        var c = new Create<int>
        {
            Name = "Create",
            Distribution = CreationDistribution,
            RequestFactory = () => Random.Shared.Next()
        };

        List<Element> list = [c];
        var d = new Dispose<int> { Name = "Dispose" };

        var processesPerBranch = Math.Max(1, n / k);

        List<Process<int>> branchHeads = [];

        for (var b = 0; b < k; b++)
        {
            SourceElement<int>? branchElement = null;
            Process<int>? firstInBranch = null;

            for (var i = 0; i < processesPerBranch; i++)
            {
                var process = new Process<int>
                {
                    Name = $"Branch {b + 1} - Proc {i + 1}",
                    Distribution = ProcessDistribution
                };

                if (i == 0)
                {
                    firstInBranch = process;
                }
                else
                {
                    branchElement!.NextElement = process;
                }

                list.Add(process);
                branchElement = process;
            }

            branchElement?.NextElement = d;

            if (firstInBranch != null)
            {
                branchHeads.Add(firstInBranch);
            }
        }

        var router = new PriorityRouter<int>
        {
            Name = "SplitRouter",
        };

        foreach (var head in branchHeads)
        {
            router.AddRoute(head, RoutePriority.Medium, 1.0 / branchHeads.Count);
        }

        c.NextElement = router;
        list.Add(router);
        list.Add(d);

        var model = new Model<int>(list)
        {
            MaxEventsLimit = e
        };

        var sw = Stopwatch.StartNew();
        model.Simulate(simulationTime);
        sw.Stop();

        Console.WriteLine($"{n,-8} | {model.TotalEventsCount,-10} | {sw.ElapsedMilliseconds,-10}");
    }
}