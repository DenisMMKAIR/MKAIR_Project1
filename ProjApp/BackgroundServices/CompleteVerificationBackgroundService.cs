using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
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

        var protocolVerificationMethodIds = protocol.VerificationMethods!
            .Select(m => m.Id)
            .ToArray();

        var dbVerifications = db.SuccessInitialVerifications
            .Include(v => v.Device)
                .ThenInclude(d => d!.DeviceType)
            .Include(v => v.Etalons)
            .Where(v => v.ProtocolNumber != null &&
                        v.OwnerINN != null &&
                        v.Worker != null &&
                        v.Location != null &&
                        v.Pressure != null &&
                        v.Temperature != null &&
                        v.Humidity != null &&
                        v.MeasurementMin != null &&
                        v.MeasurementMax != null &&
                        v.MeasurementUnit != null &&
                        v.Accuracy != null &&
                        v.Device!.DeviceType!.VerificationMethodId != null)
            .Where(v => protocolVerificationMethodIds.Any(id => id == v.Device!.DeviceType!.VerificationMethodId))
            .AsEnumerable()
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

            db.SuccessInitialVerifications.Remove(verification);
            db.Manometr1Verifications.Add(manometrVerification);
            addCount++;
        }

        await db.SaveChangesAsync();
        _logger.LogInformation("Группа {Group}. Успешно добавлено {AddCount} поверок. Не удалось обработать поверок {FailedCount}", protocol.VerificationGroup, addCount, failedCount);
    }
}
