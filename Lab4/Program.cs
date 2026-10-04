using Lab4;

var nodeCounts = new[] { 1, 5, 10, 20, 50, 100 };
var eventCounts = new[] { 10_000, 50_000, 100_000, 250_000, 500_000, 1_000_000 };

var tester = new ComplexityTester(nodeCounts, eventCounts);
tester.RunSimple(double.MaxValue);
tester.RunBranched(double.MaxValue, 5);