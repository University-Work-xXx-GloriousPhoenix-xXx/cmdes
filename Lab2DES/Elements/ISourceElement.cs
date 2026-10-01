using Lab2DES.Distributions;

namespace Lab2DES.Elements;

public interface ISourceElement
{
    void OutAct();
    IDistributionStrategy Distribution { get; set; }
}
