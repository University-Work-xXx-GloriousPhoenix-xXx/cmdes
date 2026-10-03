using Core;
using Core.Distributions;
using Core.Elements.Basic;
using Lab2;
using Lab2.Routing;

var c = new Create<int>
{
    Distribution = new ExponentialDistribution(0.5),
    Name = "CREATE",
    RequestFactory = () => Random.Shared.Next()
};

var ps = new Process<int>[]
{
    new()
    {
        Distribution = new ExponentialDistribution(1.0),
        Name = "PROCESS 1",
        MaxQueue = 1,
        MaxChannels = 1
    },
    new()
    {
        Distribution = new ExponentialDistribution(1.2),
        Name = "PROCESS 2",
        MaxQueue = 1,
        MaxChannels = 1
    },
    new()
    {
        Distribution = new ExponentialDistribution(0.8),
        Name = "PROCESS 3",
        MaxQueue = 1,
        MaxChannels = 1
    }
};

var d = new Dispose<int>
{
    Name = "DISPOSE"
};

c.NextElement = ps[0];

ps[0].NextElement = new PriorityRouter<int>([
    new Route<int>(ps[1], RoutePriority.Medium, 0.65),
    new Route<int>(ps[0], RoutePriority.Medium, 0.35)
]);

ps[1].NextElement = new PriorityRouter<int>([
    new Route<int>(ps[2], RoutePriority.Medium, 0.60),
    new Route<int>(ps[0], RoutePriority.Medium, 0.40)
]);

ps[2].NextElement = new PriorityRouter<int>([
    new Route<int>(d, RoutePriority.Medium, 0.75),
    new Route<int>(ps[1], RoutePriority.Medium, 0.25)
]);

var model = new Model<int>([c, .. ps, d]);
model.Simulate(10000.0);
StatisticsLogger<int>.Show(model);