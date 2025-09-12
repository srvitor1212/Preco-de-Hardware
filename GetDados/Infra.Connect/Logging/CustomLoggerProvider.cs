using Microsoft.Extensions.Logging;

namespace Infra.Connect.Logging;

public class CustomLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new CustomLogger(categoryName);

    public void Dispose() { }
}

public class CustomLogger(string categoryName) : ILogger
{
    private readonly string _categoryName = categoryName;

    public IDisposable BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var threadId = Thread.CurrentThread.ManagedThreadId;
        var threadName = Thread.CurrentThread.Name ?? "SemNome";
        var message = formatter(state, exception);

        Console.WriteLine($"{now} | {threadId} - {threadName} | {_categoryName} | {message}");
    }
}