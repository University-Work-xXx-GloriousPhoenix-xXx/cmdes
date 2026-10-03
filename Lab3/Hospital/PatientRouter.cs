
using Core.Hierarchy;

namespace Lab3.Hospital;

public class PatientRouter : Router<Patient>
{
    public Func<Patient, IDestinationElement<Patient>?> RoutingLogic { get; set; } = _ => null;

    public override bool InAct(Patient request)
    {
        Quantity++;
        var destination = RoutingLogic(request);
        return destination is not null && destination.InAct(request);
    }
}
