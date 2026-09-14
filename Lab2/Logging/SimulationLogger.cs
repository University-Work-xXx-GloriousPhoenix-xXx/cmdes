namespace Lab2.Logging;

public static class SimulationLogger
{
    private static readonly Lock ConsoleLock = new();
    public static void Log(string nodeName, string message, ConsoleColor color = ConsoleColor.White)
    {
        lock (ConsoleLock)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{nodeName}] {message}");
            Console.ResetColor();
        }
    }
}
