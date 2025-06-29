using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
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
        var addGoodVerificationsCommand = scope.ServiceProvider.GetRequiredService<AddInitialVerificationCommand<InitialVerification>>();
        var addFailedVerificationsCommand = scope.ServiceProvider.GetRequiredService<AddInitialVerificationCommand<InitialVerificationFailed>>();
        var jobs = await db.InitialVerificationJobs.ToListAsync();
        foreach (var job in jobs)
        {
            _logger.LogInformation("Загрузка данных за {Date}", job.Date);
            var (vrfGood, vdfFailed) = await _fgisAPI.GetInitialVerifications(job.Date);

            var saveGood = await addGoodVerificationsCommand.ExecuteAsync(vrfGood);
            _logger.LogInformation("Поверки исправных устройств {Msg}", saveGood.Message);

            var saveFailed = await addFailedVerificationsCommand.ExecuteAsync(vdfFailed);
            _logger.LogInformation("Поверки неисправных устройств {Msg}", saveFailed.Message);

            db.InitialVerificationJobs.Remove(job);
            await db.SaveChangesAsync();
            _logger.LogInformation("Загрузка данных за {Date} завершена", job.Date);
            _keeper.Signal(BackgroundEvents.DoneInitialVerificationJob);
        }
    }
}
