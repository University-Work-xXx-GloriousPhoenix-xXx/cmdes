using System.ComponentModel;

namespace Lab3.Hospital;

public static class HospitalTracker
{
    public static List<double> Type1StayTimes { get; } = [];
    public static List<double> Type2StayTimes { get; } = [];
    public static List<double> Type3StayTimes { get; } = [];

    private static double _lastLabArrivalTime = -1.0;
    public static List<double> LabArrivalIntervals { get; } = [];

    public static void Reset()
    {
        Type1StayTimes.Clear();
        Type2StayTimes.Clear();
        Type3StayTimes.Clear();
        LabArrivalIntervals.Clear();
        _lastLabArrivalTime = -1.0;
    }

    public static void RecordLabArrival(double currentTime)
    {
        if (_lastLabArrivalTime >= 0)
        {
            var interval = currentTime - _lastLabArrivalTime;
            LabArrivalIntervals.Add(interval);
        }
        _lastLabArrivalTime = currentTime;
    }

    public static void RecordPatientFinish(Patient patient, double currentTime)
    {
        if (patient.IsFinished) return;
        patient.IsFinished = true;
        patient.SystemExitTime = currentTime;

        var totalTime = patient.SystemExitTime - patient.CreationTime;

        switch (patient.OriginalType)
        {
            case PatientType.Type1:
                Type1StayTimes.Add(totalTime);
                break;
            case PatientType.Type2:
                Type2StayTimes.Add(totalTime);
                break;
            case PatientType.Type3:
                Type3StayTimes.Add(totalTime);
                break;
            default:
                throw new InvalidEnumArgumentException();
        }
    }

    public static void ShowCustomReport()
    {
        if (Type1StayTimes.Count > 0)
            Console.WriteLine($"Average time in the system (Type 1): {Type1StayTimes.Average():F2} min.");
        if (Type2StayTimes.Count > 0)
            Console.WriteLine($"Average time in the system (Type 2): {Type2StayTimes.Average():F2} min.");
        if (Type3StayTimes.Count > 0)
            Console.WriteLine($"Average time in the system (Type 3): {Type3StayTimes.Average():F2} min.");

        var output = LabArrivalIntervals.Count > 0
            ? $"Average interval between arrivals to the laboratory: {LabArrivalIntervals.Average():F2} min."
            : "No one entered the laboratory.";
        Console.WriteLine(output);
    }
}
