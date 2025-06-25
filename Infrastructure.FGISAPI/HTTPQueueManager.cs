using Microsoft.Extensions.Logging;

namespace Infrastructure.FGISAPI;

public class HTTPQueueManager
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ILogger _logger;
    private TimeSpan _delay = TimeSpan.FromSeconds(1);
    private DateTime _lastRequest;

    public HTTPQueueManager(ILogger logger)
    {
        _logger = logger;
    }

    public void SetDelay(TimeSpan delay)
    {
        _logger.LogInformation("Delay set to {Delay}", _delay);
        _delay = delay;
    }

    public async Task WaitForQueue()
    {
        _logger.LogDebug("Waiting for queue.");
        await _semaphore.WaitAsync();
        try
        {
            if (_delay != TimeSpan.Zero)
            {
                var currentTime = DateTime.UtcNow;
                var timeSinceLastAccess = currentTime - _lastRequest;
                if (timeSinceLastAccess < _delay)
                {
                    var remainingDelay = _delay - timeSinceLastAccess;
                    _logger.LogDebug("Waiting {RemainingDelay} before making request.", remainingDelay);
                    await Task.Delay(remainingDelay);
                }
            }
        }
        finally
        {
            _lastRequest = DateTime.UtcNow;
            _semaphore.Release();
        }
    }
}
