namespace Lab2.SimulationUtils;

public class Request : IDisposable
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}