using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
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
        await ProcessDavlenie1Async(db);
    }

    private async Task<(IReadOnlyList<SuccessInitialVerification>, IReadOnlyList<Owner>)> GetVerificationsAsync(ProjDatabase db, Expression<Func<SuccessInitialVerification, bool>> predicate)
    {
        var vrfs = await db.SuccessInitialVerifications
               .Include(v => v.Device)
                   .ThenInclude(d => d!.DeviceType)
               .Include(v => v.VerificationMethod)
                   .ThenInclude(vm => vm!.ProtocolTemplate)
               .Include(v => v.Etalons)
               .VerificationIsFilled()
               .Where(predicate)
               .ToArrayAsync();

        var ownerNames = vrfs.Select(v => v.Owner).Distinct().ToArray();

        var owners = await db.Owners
            .Where(o => ownerNames.Contains(o.Name))
            .ToArrayAsync();

        return (vrfs, owners);
    }

    private async Task ProcessManometr1Async(ProjDatabase db)
    {
        Expression<Func<SuccessInitialVerification, bool>> predicate = vrf
            => vrf.VerificationGroup == VerificationGroup.Манометры &&
               vrf.VerificationMethod!.ProtocolTemplate!.ProtocolGroup == ProtocolGroup.Манометр1;

        var (verifications, owners) = await GetVerificationsAsync(db, predicate);

        if (verifications.Count == 0) return;

        foreach (var vrf in verifications)
        {
            var owner = owners.FirstOrDefault(o => o.Name == vrf.Owner)
                ?? throw new Exception($"Не найден владелец {vrf.Owner}");
            var manometrVrf = Manometr1Calculations.ToManometr1(_logger, vrf, owner);
            db.SuccessInitialVerifications.Remove(vrf);
            db.Manometr1Verifications.Add(manometrVrf);
        }

        await db.SaveChangesAsync();

        var group = verifications[0].VerificationGroup!;
        _logger.LogInformation("Группа {Group}. Добавлено {AddCount} поверок", group, verifications.Count);
    }

    private async Task ProcessDavlenie1Async(ProjDatabase db)
    {
        Expression<Func<SuccessInitialVerification, bool>> predicate = vrf
            => vrf.VerificationGroup == VerificationGroup.Датчики_давления &&
               vrf.VerificationMethod!.ProtocolTemplate!.ProtocolGroup == ProtocolGroup.Давление1;

        var (verifications, owners) = await GetVerificationsAsync(db, predicate);

        if (verifications.Count == 0) return;

        foreach (var vrf in verifications)
        {
            var owner = owners.FirstOrDefault(o => o.Name == vrf.Owner)
                ?? throw new Exception($"Не найден владелец {vrf.Owner}");
            var davlenieVrf = Davlenie1Calculations.ToDavlenie1(_logger, vrf, owner);
            db.SuccessInitialVerifications.Remove(vrf);
            db.Davlenie1Verifications.Add(davlenieVrf);
        }

        await db.SaveChangesAsync();

        var group = verifications[0].VerificationGroup!;
        _logger.LogInformation("Группа {Group}. Добавлено {AddCount} поверок", group, verifications.Count);
    }
}
