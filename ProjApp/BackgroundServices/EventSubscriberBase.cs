using Microsoft.Extensions.Logging;

namespace ProjApp.BackgroundServices;

public abstract class EventSubscriberBase : IDisposable
{
    private readonly ILogger _logger;
    private int _state; // 0=idle, 1=processing, 2=processing_with_pending
    private readonly List<IDisposable> _subscriptions = [];
    private bool _disposed;

    public EventSubscriberBase(ILogger logger) => _logger = logger;

    public virtual void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }
        GC.SuppressFinalize(this);
    }

    protected void SubscribeTo(EventKeeper keeper, BackgroundEvents eventName)
    {
        _subscriptions.Add(keeper.Subscribe(eventName, OnEventTriggered));
    }

    protected void OnEventTriggered()
    {
        // Fast path: If already processing, upgrade to "processing_with_pending"
        if (Interlocked.CompareExchange(ref _state, 2, 1) == 1)
            return;

        // Try to start processing (state must be 0/idle)
        if (Interlocked.CompareExchange(ref _state, 1, 0) == 0)
            _ = ProcessAsync(); // Fire and forget
    }

    protected abstract Task ProcessWorkAsync();

    private async Task ProcessAsync()
    {
        do
        {
            try { await ProcessWorkAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Processing error"); }

            // Atomically check if we should continue or exit
            int currentState = Interlocked.CompareExchange(ref _state, 0, 1);

            if (currentState == 2)
            {
                // Pending work exists, transition back to processing (state=1)
                Interlocked.CompareExchange(ref _state, 1, 2);
                continue;
            }
            break;
        } while (true);
    }
}
