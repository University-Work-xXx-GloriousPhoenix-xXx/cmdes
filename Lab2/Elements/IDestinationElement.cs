namespace Lab2DES.Elements;
public interface IDestinationElement<in TRequest>
{
    bool InAct(TRequest request);
}