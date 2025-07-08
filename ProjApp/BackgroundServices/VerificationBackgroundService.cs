using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;

namespace ProjApp.BackgroundServices;

public class VerificationBackgroundService : EventSubscriberBase, IHostedService
{
    private readonly ILogger<VerificationBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly EventKeeper _keeper;

    public VerificationBackgroundService(ILogger<VerificationBackgroundService> logger,
                                                IServiceScopeFactory serviceScopeFactory,
                                                EventKeeper eventKeeper) : base(logger)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _keeper = eventKeeper;
        SubscribeTo(_keeper, BackgroundEvents.ChangedOwner);
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
        using var scope = _serviceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
        var addSVCommand = scope.ServiceProvider.GetRequiredService<AddVerificationCommand<SuccessVerification>>();
        var addFVCommand = scope.ServiceProvider.GetRequiredService<AddVerificationCommand<FailedVerification>>();

        var initialVS = await db.InitialVerificationsSuccess
            .Include(v => v.Device)
                .ThenInclude(d => d!.DeviceType)
            .Include(v => v.Etalons)
            .Where(v => v.VerificationTypeNum != null &&
                        v.OwnerInn != null &&
                        v.Worker != null &&
                        v.Location != null &&
                        v.AdditionalInfo != null &&
                        v.Pressure != null &&
                        v.Temperature != null &&
                        v.Humidity != null)
            .ToArrayAsync();

        var initialVF = await db.InitialVerificationsFailed
            .Include(v => v.Device)
                .ThenInclude(d => d!.DeviceType)
            .Include(v => v.Etalons)
            .Where(v => v.VerificationTypeNum != null &&
                        v.OwnerInn != null &&
                        v.Worker != null &&
                        v.Location != null &&
                        v.AdditionalInfo != null &&
                        v.Pressure != null &&
                        v.Temperature != null &&
                        v.Humidity != null)
            .ToArrayAsync();

        if (initialVS.Length == 0 && initialVF.Length == 0) return;

        using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            db.RemoveRange(initialVS);

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка добавления заполненных поверок");
            await transaction.RollbackAsync();
        }

        var vs = initialVS.Select(ToSV).ToArray();
        var resultSV = await addSVCommand.ExecuteAsync(vs);
        _logger.LogInformation("Добавлено пройденных заполненных поверок {NewCount}. Отсеяно дубликатов {DuplicateCount}", resultSV.NewCount, resultSV.DuplicateCount);

        var vf = initialVF.Select(ToFV).ToArray();
        var resultFV = await addFVCommand.ExecuteAsync(vf);
        _logger.LogInformation("Добавлено непройденных заполненных поверок {NewCount}. Отсеяно дубликатов {DuplicateCount}", resultFV.NewCount, resultFV.DuplicateCount);
    }

    private static SuccessVerification ToSV(SuccessInitialVerification v)
    {
        return new()
        {
            DeviceTypeNumber = v.DeviceTypeNumber,
            DeviceSerial = v.DeviceSerial,
            Owner = v.Owner,
            VerificationTypeNames = v.VerificationTypeNames,
            VerificationDate = v.VerificationDate,
            VerifiedUntilDate = v.VerifiedUntilDate,
            VerificationTypeNum = v.VerificationTypeNum!,
            OwnerInn = v.OwnerInn!.Value,
            Worker = v.Worker!,
            Location = v.Location!.Value,
            AdditionalInfo = v.AdditionalInfo!,
            Pressure = v.Pressure!,
            Temperature = v.Temperature!.Value,
            Humidity = v.Humidity!.Value,

            Device = v.Device,
            Etalons = v.Etalons
        };
    }

    private static FailedVerification ToFV(FailedInitialVerification v)
    {
        return new()
        {
            DeviceTypeNumber = v.DeviceTypeNumber,
            DeviceSerial = v.DeviceSerial,
            Owner = v.Owner,
            VerificationTypeNames = v.VerificationTypeNames,
            VerificationDate = v.VerificationDate,
            FailedDocNumber = v.FailedDocNumber,
            VerificationTypeNum = v.VerificationTypeNum!,
            OwnerInn = v.OwnerInn!.Value,
            Worker = v.Worker!,
            Location = v.Location!.Value,
            AdditionalInfo = v.AdditionalInfo!,
            Pressure = v.Pressure!,
            Temperature = v.Temperature!.Value,
            Humidity = v.Humidity!.Value,
            Device = v.Device,
            Etalons = v.Etalons
        };
    }
}
