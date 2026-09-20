namespace Lab2.SimulationUtils;

public record SimulationResult(
    int RunNumber,

    double AverageArrivalDelay,
    int P1Devices,
    int P1Queue,
    double P1Time,
    int P2Devices,
    int P2Queue,
    double P2Time,
    int P3Devices,
    int P3Queue,
    double P3Time,
    double RouteProbabilityP1ToP2,

    long TotalIncomingRequests,
    long P1Unprocessed,
    long P2Unprocessed,
    long P3Unprocessed,
    double RejectionProbability,
    double P1AverageQueueLength,
    double P1AverageBusyDevices,
    double P2AverageQueueLength,
    double P2AverageBusyDevices,
    double P3AverageQueueLength,
    double P3AverageBusyDevices
);