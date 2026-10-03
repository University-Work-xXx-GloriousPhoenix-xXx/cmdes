using Core.Hierarchy;

namespace Core.Elements.Basic;

public class Create<TRequest> : SourceElement<TRequest>
{
    public required Func<TRequest> RequestFactory { get; set; }
    public Create()
    {
        TNext = 0.0;
    }

    public override void OutAct()
    {
        Quantity++;
        TNext = TCurr + GetDelay();

        var request = RequestFactory();
        NextElement?.InAct(request);
    }
}
