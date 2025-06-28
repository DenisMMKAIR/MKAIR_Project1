using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Commands;
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
        var addCommand = scope.ServiceProvider.GetRequiredService<AddInitialVerificationCommand>();
        var jobs = await db.InitialVerificationJobs.ToListAsync();
        foreach (var job in jobs)
        {
            _logger.LogInformation("Загрузка данных за {Date}", job.Date);
            var initialVerifications = await _fgisAPI.GetInitialVerifications(job.Date);
            var saveResult = await addCommand.ExecuteAsync(initialVerifications);
            _logger.LogInformation("{Msg}", saveResult.Message);
            db.InitialVerificationJobs.Remove(job);
            await db.SaveChangesAsync();
            _logger.LogInformation("Загрузка данных за {Date} завершена", job.Date);
            _keeper.Signal(BackgroundEvents.DoneInitialVerificationJob);
        }
    }
}
