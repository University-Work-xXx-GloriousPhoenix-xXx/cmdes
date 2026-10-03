using Lab2DES.Elements;

namespace Lab3;

public class StateModifier<TRequest> : Element, IDestinationElement<TRequest>
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