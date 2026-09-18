using System.Collections.Concurrent;

namespace Lab2.SimulationNodes;

public class Dispose(double serviceTime) : IReceiverNode
{
    private readonly ConcurrentQueue<Request> _requestQueue = [];

    public void ProcessRequest(Request request) => _requestQueue.Enqueue(request);

    public async Task RunAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            while (_requestQueue.IsEmpty && !ct.IsCancellationRequested)
            {
                await Task.Delay(50, ct);
            }

            if (ct.IsCancellationRequested) break;

            if (!_requestQueue.TryDequeue(out var request))
            {
                continue;
            }

            await Task.Delay(TimeSpan.FromSeconds(serviceTime), ct);

            request.Dispose();
        }
    }
}