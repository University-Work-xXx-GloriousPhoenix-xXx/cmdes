namespace Lab3.Hospital;

public class Patient(PatientType type)
{
    public PatientType OriginalType { get; init; } = type;
    public PatientType Type { get; set; } = type;
    public double CreationTime { get; set; } = double.NegativeInfinity;
    public double SystemExitTime { get; set; }
    public bool IsFinished { get; set; }
}