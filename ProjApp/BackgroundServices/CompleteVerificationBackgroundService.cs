using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

    protected override Task ProcessWorkAsync()
    {
        /*
        using var scope = _serviceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();

        var protocolTemplates = await db.ProtocolTemplates
                             .Include(pt => pt.VerificationMethods)
                             .Include(pt => pt.CompleteSuccessVerifications)
                             .Include(pt => pt.CompleteFailVerifications)
                             .ToArrayAsync();

        foreach (var pt in protocolTemplates)
        {
            var aliases = pt.VerificationMethods!.SelectMany(vm => vm.Aliases).ToArray();

            var ivs = db.InitialVerificationsSuccess
                .Where(iv => aliases.All(a => iv.VerificationTypeNames.Contains(a)))
                .Where(iv => pt.DeviceTypeNumbers.Contains(iv.DeviceTypeNumber))
                .ToArray();

            using var transaction = db.Database.BeginTransaction();

            try
            {
                db.InitialVerificationsSuccess.RemoveRange(ivs);
                var cvs = ivs.Select(iv => { iv.Id = new Guid(); return iv; }).ToArray();
                db.VerificationsSuccess.AddRange(ivs);

                cvs = db.VerificationsSuccess
                        .Where(cv => cvs.Contains(cv, new InitialVerificationUniqComparer<SuccessInitialVerification>()))
                        .ToArray();

                pt.CompleteSuccessVerifications = pt.CompleteSuccessVerifications!.Concat(cvs).ToList();
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка добавления готовых проверок");
                await transaction.RollbackAsync();
            }

            //TODO: implement
            throw new NotImplementedException();
        }
        */
        //TODO: implement
        throw new NotImplementedException();
    }
}
