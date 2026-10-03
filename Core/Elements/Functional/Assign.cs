using Core.Hierarchy;

namespace Core.Elements.Functional;

public class Assign<TRequest> : Element, IDestinationElement<TRequest>
{
    public Action<TRequest> ModifyAction { get; set; } = _ => { };

    public IDestinationElement<TRequest>? NextElement { get; set; }

    public bool InAct(TRequest request)
    {
        Quantity++;
        ModifyAction(request);
return NextElement == null || NextElement.InAct(request);
    }
}