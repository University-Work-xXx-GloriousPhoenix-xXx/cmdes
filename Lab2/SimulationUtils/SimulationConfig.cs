namespace Lab2.SimulationUtils;

public record SimulationConfig(
    int RequestCount,
    double AverageArrivalDelay,
    List<(int DeviceCount, int QueueLimit, double AverageServiceTime)> QueuingSystemsDataList);
