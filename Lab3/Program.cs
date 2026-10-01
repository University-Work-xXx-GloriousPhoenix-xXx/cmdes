using Lab2DES.Distributions;
using Lab2DES.Elements;
using Lab3;

var c = new Create
{
    Name = "Create",
    Distribution = new ExponentialDistribution(0.5)
};
c.TNext = 0.1;

var p1 = new Process
{
    Name = "Lane A",
    Distribution = new ExponentialDistribution(0.3),
    MaxChannels = 1,
    MaxQueue = 3
};
var p2 = new Process
{
    Name = "Lane B",
    Distribution = new ExponentialDistribution(0.3),
    MaxChannels = 1,
    MaxQueue = 3
};
var bp = new BankProcess(p1, p2);

var d = new Dispose
{
    Name = "Dispose"
};

c.NextElement = bp;
p1.NextElement = d;
p2.NextElement = d;

var model = new BankModel([c, p1, p2, bp, d], bp);
model.Simulate(10000);
model.ShowBankReport(bp);
