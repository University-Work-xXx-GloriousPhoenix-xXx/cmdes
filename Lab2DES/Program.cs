using Lab2DES;
using Lab2DES.Distributions;
using Lab2DES.Elements;
using Lab2DES.Routing;

var c = new Create
{
    Distribution = new ExponentialDistribution(0.5),
    Name = "CREATE"
};

var ps = new Process[]
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

var d = new Dispose
{
    Name = "DISPOSE"
};

c.NextElement = ps[0];

ps[0].NextElement = new PriorityRouter([
    new Route(ps[1], RoutePriority.Medium, 0.65),
    new Route(ps[0], RoutePriority.Medium, 0.35)
]);

ps[1].NextElement = new PriorityRouter([
    new Route(ps[2], RoutePriority.Medium, 0.60),
    new Route(ps[0], RoutePriority.Medium, 0.40)
]);

ps[2].NextElement = new PriorityRouter([
    new Route(d, RoutePriority.Medium, 0.75),
    new Route(ps[1], RoutePriority.Medium, 0.25)
]);

var model = new Model([c, .. ps, d]);
model.Simulate(10000.0).Show();