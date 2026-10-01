using Lab2DES;
using Lab2DES.Distributions;
using Lab2DES.Elements;
using Lab2DES.Routing;

var c = new Create
{
    Distribution = new ExponentialDistribution(0.3),
    Name = "CREATE"
};

var ps = new Process[]
{
    new()
    {
        Distribution = new ExponentialDistribution(1.0),
        Name = "PRC: Dispatcher",
        MaxQueue = 2,
        MaxChannels = 1
    },
    new()
    {
        Distribution = new ExponentialDistribution(2.0),
        Name = "PRC: Fast Lane (HIGH Priority)",
        MaxQueue = 0,
        MaxChannels = 1
    },
    new()
    {
        Distribution = new ExponentialDistribution(1.0),
        Name = "PRC: Backup A (MED 50%)",
        MaxQueue = 3,
        MaxChannels = 1
    },
    new()
    {
        Distribution = new ExponentialDistribution(1.0),
        Name = "PRC: Backup B (MED 50%)",
        MaxQueue = 3,
        MaxChannels = 1
    }
};

var d = new Dispose
{
    Name = "DISPOSE"
};

c.AddRoute(ps[0], RoutePriority.High, 1.0);

ps[0].AddRoute(ps[1], RoutePriority.High, 1.0);

ps[0].AddRoute(ps[2], RoutePriority.Medium, 0.50);
ps[0].AddRoute(ps[3], RoutePriority.Medium, 0.50);

ps[1].AddRoute(d, RoutePriority.Medium, 1.0);
ps[2].AddRoute(d, RoutePriority.Medium, 1.0);
ps[3].AddRoute(d, RoutePriority.Medium, 1.0);

var model = new Model([c, .. ps, d]);
model.Simulate(10000.0).Show();