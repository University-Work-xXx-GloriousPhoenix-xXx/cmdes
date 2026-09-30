
using Lab2DES;
using Lab2DES.Distributions;
using Lab2DES.Elements;

var c = new Create
{
    Distribution = new ExponentialDistribution(2.0),
    Name = "CREATE"
};

var ps = new Process[]
{
    new()
    {
        Distribution = new ExponentialDistribution(1.0),
        Name = "PROCESS 1",
        MaxQueue = 5
    },
    new()
    {
        Distribution = new ExponentialDistribution(1.0),
        Name = "PROCESS 2",
        MaxQueue = 3
    },
    new()
    {
        Distribution = new ExponentialDistribution(1.0),
        Name = "PROCESS 3",
        MaxQueue = 4
    },
};

var d = new Dispose
{
    Distribution = new DefinedDistribution(0.0),
    Name = "DISPOSE"
};

c.NextElement = ps[0];
ps[0].NextElement = ps[1];
ps[1].NextElement = ps[2];
ps[2].NextElement = d;

var model = new Model([c, .. ps, d]);

model.Simulate(10000.0).Show();
