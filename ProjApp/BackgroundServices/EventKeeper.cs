using Microsoft.Extensions.Logging;

namespace ProjApp.BackgroundServices;

public class EventKeeper
{
    private readonly Dictionary<BackgroundEvents, List<Action>> _eventHandlers = [];
    private readonly ReaderWriterLockSlim _lock = new();

    private readonly ILogger<EventKeeper> _logger;

    public EventKeeper(ILogger<EventKeeper> logger) => _logger = logger;

    public IDisposable Subscribe(BackgroundEvents eventName, Action handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _lock.EnterWriteLock();
        try
        {
            if (!_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                handlers = [];
                _eventHandlers[eventName] = handlers;
            }
            handlers.Add(handler);

            return new SubscriptionToken(() => Unsubscribe(eventName, handler));
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Unsubscribe(BackgroundEvents eventName, Action handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _lock.EnterWriteLock();
        try
        {
            if (_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                handlers.Remove(handler);
                if (handlers.Count == 0)
                {
                    _eventHandlers.Remove(eventName);
                }
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Signal(BackgroundEvents eventName)
    {
        Action[] handlersCopy;

        _lock.EnterReadLock();
        try
        {
            if (!_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                return;
            }
            handlersCopy = [.. handlers];
        }
        finally
        {
            _lock.ExitReadLock();
        }

        foreach (var handler in handlersCopy)
        {
            try
            {
                handler.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invoking event handler: {Msg}", ex.Message);
            }
        }
    }

    private class SubscriptionToken : IDisposable
    {
        private Action? _unsubscribeAction;
        private int _disposed;

        public SubscriptionToken(Action unsubscribeAction)
        {
            _unsubscribeAction = unsubscribeAction ?? throw new ArgumentNullException(nameof(unsubscribeAction));
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _unsubscribeAction?.Invoke();
                _unsubscribeAction = null;
            }
        }
    }
}
