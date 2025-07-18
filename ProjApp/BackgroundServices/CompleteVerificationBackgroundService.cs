using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.InfrastructureInterfaces;
using ProjApp.ProtocolCalculations;

namespace ProjApp.BackgroundServices;

public class CompleteVerificationBackgroundService : EventSubscriberBase, IHostedService
{
    private readonly ILogger<CompleteVerificationBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly EventKeeper _keeper;

    public CompleteVerificationBackgroundService(ILogger<CompleteVerificationBackgroundService> logger,
                                                IServiceScopeFactory serviceScopeFactory,
                                                EventKeeper eventKeeper) : base(logger)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _keeper = eventKeeper;
        SubscribeTo(_keeper, BackgroundEvents.NewProtocolTemplate);
        SubscribeTo(_keeper, BackgroundEvents.ChangedProtocolTemplate);
        SubscribeTo(_keeper, BackgroundEvents.AddedValuesInitialVerification);
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
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
        
        await ProcessManometr1Async(db);
    }

    private async Task ProcessManometr1Async(ProjDatabase db)
    {
        var verifications = await db.SuccessInitialVerifications
            .Include(v => v.Device)
                .ThenInclude(d => d!.DeviceType)
            .Include(v => v.VerificationMethod)
                .ThenInclude(vm => vm!.ProtocolTemplate)
            .Include(v => v.Etalons)
            .VerificationIsFilled()
            .Where(v => v.VerificationMethod!.ProtocolTemplate!.ProtocolGroup == ProtocolGroup.Манометр1)
            .ToArrayAsync();

        if (verifications.Length == 0) return;

        foreach (var vrf in verifications)
        {
            var manometrVrf = Manometr1Calculations.ToManometr1(_logger, vrf);
            db.SuccessInitialVerifications.Remove(vrf);
            db.Manometr1Verifications.Add(manometrVrf);
        }

        await db.SaveChangesAsync();

        var group = verifications.First().VerificationGroup!;
        _logger.LogInformation("Группа {Group}. Добавлено {AddCount} поверок", group, verifications.Length);
    }
}
