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
        Name = "PRC: Primary",
        MaxQueue = 1,
        MaxChannels = 1
    },
    new()
    {
        Distribution = new ExponentialDistribution(1.2),
        Name = "PRC: Heavy Assembly",
        MaxQueue = 1,
        MaxChannels = 1
    },
    new()
    {
        Distribution = new ExponentialDistribution(0.8),
        Name = "PRC: Quality Control",
        MaxQueue = 1,
        MaxChannels = 1
    }
};

var d = new Dispose
{
    Name = "DISPOSE"
};

c.SetRoutes(ps[0], RoutePriority.High, 1.0);

ps[0]
    .AddRoute(ps[1], RoutePriority.Medium, 0.65)
    .AddRoute(ps[0], RoutePriority.Medium, 0.35);

ps[1]
    .AddRoute(ps[2], RoutePriority.Medium, 0.60)
    .AddRoute(ps[0], RoutePriority.Medium, 0.40);

ps[2]
    .AddRoute(d, RoutePriority.Medium, 0.75)
    .AddRoute(ps[1], RoutePriority.Medium, 0.25);

var model = new Model([c, .. ps, d]);
model.Simulate(10000.0).Show();