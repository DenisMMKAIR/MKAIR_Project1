using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Services;

namespace ProjApp.BackgroundServices;

[Obsolete("Rewrite to transaction use")]
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
        SubscribeTo(_keeper, BackgroundEvents.NewInitialVerificationJob);
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
        var addGoodVerificationsCommand = scope.ServiceProvider.GetRequiredService<AddInitialVerificationCommand<SuccessInitialVerification>>();
        var addFailedVerificationsCommand = scope.ServiceProvider.GetRequiredService<AddInitialVerificationCommand<FailedInitialVerification>>();
        var vService = scope.ServiceProvider.GetRequiredService<VerificationsService>();
        var jobs = await db.InitialVerificationJobs.ToListAsync();
        try
        {
            foreach (var job in jobs)
            {
                _logger.LogInformation("Загрузка данных за {Date}", job.Date);
                var result = await _fgisAPI.GetInitialVerificationsAsync(job.Date);

                if (result.Error != null)
                {
                    _logger.LogError("{Error}", result.Error);
                    continue;
                }

                _logger.LogInformation("Сохранение исправных устройств");
                var saveGood = await addGoodVerificationsCommand.ExecuteAsync(result.SuccessInitialVerifications!);
                _logger.LogInformation("Поверки исправных устройств {Msg}", saveGood.Message);

                _logger.LogInformation("Сохранение неисправных устройств");
                var saveFailed = await addFailedVerificationsCommand.ExecuteAsync(result.FailedInitialVerifications!);
                _logger.LogInformation("Поверки неисправных устройств {Msg}", saveFailed.Message);

                _ = await vService.AddVerificationMethodsAsync(result.SuccessInitialVerifications!);
                _ = await vService.AddVerificationMethodsAsync(result.FailedInitialVerifications!);

                db.InitialVerificationJobs.Remove(job);
                await db.SaveChangesAsync();
                _logger.LogInformation("Загрузка данных за {Date} завершена", job.Date);
                _keeper.Signal(BackgroundEvents.DoneInitialVerificationJob);
            }
        }
        catch (Exception e)
        {
            _logger.LogError("{Msg}", e.Message);
        }
    }
}
