namespace Core.Hierarchy;
public interface IDestinationElement<in TRequest>
{
    bool InAct(TRequest request);
}