namespace Up.Bsky.PostBot.Util;

public abstract class DelayedService<T> : IHostedService, IDisposable where T : DelayedService<T>
{
    private readonly TimeSpan _updateDelay;
    private readonly TimeSpan _startDelay;
    private Timer? _timer;
    
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ILogger<T> Logger;

    protected DelayedService(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, TimeSpan updateDelay, TimeSpan startDelay = default)
    {
        ServiceProvider = serviceProvider;
        Logger = loggerFactory.CreateLogger<T>();
        _updateDelay = updateDelay;
        _startDelay = startDelay;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(state => DoWork(state, cancellationToken), null, _startDelay, _updateDelay);
        return Task.CompletedTask;
    }
    
    private void DoWork(object? state, CancellationToken cancellationToken)
    {
        using var scope = ServiceProvider.CreateScope();
        using (Logger.BeginScope("{Service}.Run", typeof(T).Name))
        {
            Logger.LogInformation("Executing Timer Task");
            try
            {
                Run(scope, state, cancellationToken).Wait(cancellationToken);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to execute {Service}", typeof(T).Name);
            }
        }
    }

    protected abstract Task Run(IServiceScope scope, object? state, CancellationToken cancellationToken);
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
        GC.SuppressFinalize(this);
    }
}
