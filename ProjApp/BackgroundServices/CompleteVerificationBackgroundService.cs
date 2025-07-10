using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Commands;
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
        using var scopre = _serviceScopeFactory.CreateScope();
        var db = scopre.ServiceProvider.GetRequiredService<ProjDatabase>();
        var pdfCreator = scopre.ServiceProvider.GetRequiredService<ITemplateProcessor>();

        await ProcessManometr1Async(db, pdfCreator);
    }

    private async Task ProcessManometr1Async(ProjDatabase db, ITemplateProcessor pdfCreator)
    {
        var protocol = await db.ProtocolTemplates
            .Include(p => p.VerificationMethods)
            .FirstAsync(p => p.ProtocolGroup == ProtocolGroup.Манометр1);

        // TODO: Not sure we have to check this. Initial verifications already checked before add
        var existsMan1 = await db.Manometr1Verifications
            .ProjectToType<IVerificationBase>()
            .ToArrayAsync();

        var dbVerifications = db.SuccessVerifications
            .Include(v => v.Device)
                .ThenInclude(d => d!.DeviceType)
            .Include(v => v.Etalons)
            .Where(v => v.VerificationGroup == protocol.VerificationGroup)
            .AsEnumerable()
            .Where(v => !existsMan1.Contains(v, VerificationUniqComparer.Instance))
            .Where(v => protocol.VerificationMethods!.Any(m => m.Aliases.Contains(v.VerificationTypeName)))
            .ToArray();

        var dbVerificationMethods = db.VerificationMethods
            .AsEnumerable()
            .Where(m => m.Aliases.Any(a => dbVerifications.Any(v => a == v.VerificationTypeName)))
            .ToArray();

        var addCount = 0;
        var failedCount = 0;

        foreach (var verification in dbVerifications)
        {
            var manometrVerification = Manometr1Calculations.ToManometr1(_logger, verification, dbVerificationMethods);

            var result = await pdfCreator.CreatePDFAsync(protocol, manometrVerification);

            if (result.Error != null)
            {
                _logger.LogError("Группа {Group}. Поверка {Verification}. Не удалось создать pdf и завершить регистрацию поверки. {Error}",
                                 verification.VerificationGroup, verification, result.Error);

                failedCount++;
                continue;
            }

            db.SuccessVerifications.Remove(verification);
            db.Manometr1Verifications.Add(manometrVerification);
            addCount++;
        }

        await db.SaveChangesAsync();
        _logger.LogInformation("Группа {Group}. Успешно добавлено {AddCount} поверок. Не удалось обработать поверок {FailedCount}", protocol.VerificationGroup, addCount, failedCount);
    }
}
