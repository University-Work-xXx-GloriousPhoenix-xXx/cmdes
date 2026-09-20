using Lab2.SimulationHandlers;
using Lab2.SimulationUtils;

const int baseRequests = 1000;
var runConfigs = new SimulationConfig[]{
    new(baseRequests, 0.2, [(5, 10, 1.2), (7, 8, 2.0), (2, 1, 1.0)]),
    new((int)(baseRequests / 2.0), 0.4, [(5, 10, 1.2), (7, 8, 2.0), (2, 1, 1.0)]),
    new(baseRequests, 0.2, [(5, 10, 0.6), (7, 8, 2.0), (2, 1, 1.0)]),
    new(baseRequests, 0.2, [(5, 10, 1.2), (7, 9, 2.0), (2, 1, 1.0)]),
    new(baseRequests, 0.2, [(5, 10, 1.2), (7, 8, 2.0), (3, 1, 1.0)]),
    new(baseRequests, 0.2, [(9, 10, 1.2), (7, 9, 2.0), (3, 1, 1.0)]),
    new((int)(baseRequests / 10.0), 2.0, [(9, 0, 1.2), (7, 9, 2.0), (3, 1, 1.0)]),
    new((int)(baseRequests / 10.0), 2.0, [(3, 0, 1.2), (7, 0, 2.0), (3, 0, 1.0)]),
    new(baseRequests, 0.2, [(5, 10, 1.2), (7, 2, 2.0), (3, 1, 1.0)]),
    new(baseRequests, 0.2, [(5, 10, 1.2), (7, 2, 2.0), (3, 1, 1.0)])
};

await SimulationOrchestrator.RunAsync(runConfigs, "outs");