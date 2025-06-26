using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.InfrastructureInterfaces;

namespace ProjApp.BackgroundServices;

public class InitialVerificationBackgroundService : EventSubscriberBase, IHostedService
{
    private readonly ILogger<InitialVerificationBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IFGISAPI _fgisAPI;
    private readonly EventKeeper _keeper;

    public InitialVerificationBackgroundService(ILogger<InitialVerificationBackgroundService> logger,
                                                IServiceScopeFactory serviceScopeFactory,
                                                IFGISAPI fGISAPI,
                                                EventKeeper eventKeeper) : base(logger)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _fgisAPI = fGISAPI;
        _keeper = eventKeeper;
        eventKeeper.Subscribe(BackgroundEvents.NewInitialVerificationJob, OnEventTriggered);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        OnEventTriggered();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override async Task ProcessWorkAsync()
    {
        using var scope = _serviceScopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
        var jobs = await db.InitialVerificationJobs.ToListAsync();
        foreach (var job in jobs)
        {
            _logger.LogInformation("Processing initial verification job {Job}. ProcessedCount {ProcessedCount}",
                job.Date, job.LoadedVerifications);

            var result = await _fgisAPI.GetInitialVerifications(job.Date);
            
            _keeper.Signal(BackgroundEvents.DoneInitialVerificationJob);
        }
    }
}
