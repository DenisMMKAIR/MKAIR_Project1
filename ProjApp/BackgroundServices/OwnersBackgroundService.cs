using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Entities;
using ProjApp.Normalizers;

namespace ProjApp.BackgroundServices;

public class OwnersBackgroundService : EventSubscriberBase, IHostedService
{
    private readonly ILogger<OwnersBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly EventKeeper _keeper;

    public OwnersBackgroundService(ILogger<OwnersBackgroundService> logger,
                                   IServiceScopeFactory serviceScopeFactory,
                                   EventKeeper eventKeeper) : base(logger)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _keeper = eventKeeper;
        SubscribeTo(_keeper, BackgroundEvents.DoneInitialVerificationJob);
        SubscribeTo(_keeper, BackgroundEvents.ChangedOwner);
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
        using var scope = _serviceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
        var n = new OwnerNameNormalizer();

        var normOwnerNames = db.SuccessInitialVerifications
            .Select(iv => iv.Owner)
            .Union(db.FailedInitialVerifications
                     .Select(iv => iv.Owner))
            .Distinct()
            .AsEnumerable()
            .Select(n.Normalize)
            .ToImmutableSortedSet();

        var dbOwners = await db.Owners
            .Where(o => o.INN != 0 && normOwnerNames.Contains(o.Name))
            .ToArrayAsync();

        var ivc = db.SuccessInitialVerifications
            .AsEnumerable()
            .Where(iv => dbOwners.Any(dbo => dbo.Name.Contains(n.Normalize(iv.Owner))))
            .ToArray();

        var ivf = db.FailedInitialVerifications
            .AsEnumerable()
            .Where(iv => dbOwners.Any(dbo => dbo.Name.Contains(n.Normalize(iv.Owner))))
            .ToArray();

        using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            uint set = 0, miss = 0;

            foreach (var iv in ((IReadOnlyList<IInitialVerification>)ivc)
                               .Union(ivf))
            {
                var owner = dbOwners.FirstOrDefault(o => o.Name == n.Normalize(iv.Owner));
                if (owner == null)
                {
                    miss++;
                    continue;
                }
                if (iv.OwnerINN == owner.INN) continue;
                iv.OwnerINN = owner.INN;
                set++;
            }

            if (set == 0 && miss == 0) return;

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Поверок обновлено: {set}. Владельцев не найдено: {miss}", set, miss);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка добавления инн владельцев поверкам. {Msg}", e.Message);
            await transaction.RollbackAsync();
        }
    }
}