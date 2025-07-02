using Microsoft.Extensions.Logging;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class PendingManometrVerificationsService
{
    private readonly ILogger<PendingManometrVerificationsService> _logger;
    private readonly IPendingManometrVerificationsProcessor _pendingVerificationsProcessor;
    private readonly ProjDatabase _database;
    private readonly AddPendingManometrCommand _addCommand;
    private readonly EventKeeper _keeper;

    public PendingManometrVerificationsService(ILogger<PendingManometrVerificationsService> logger,
        IPendingManometrVerificationsProcessor pendingVerificationsProcessor,
        ProjDatabase database,
        AddPendingManometrCommand addCommand,
        EventKeeper keeper)
    {
        _logger = logger;
        _pendingVerificationsProcessor = pendingVerificationsProcessor;
        _database = database;
        _addCommand = addCommand;
        _keeper = keeper;
    }

    public async Task<ServicePaginatedResult<PendingManometrVerification>> GetPaginatedAsync(int pageIndex, int pageSize)
    {
        var result = await _database.PendingManometrVerifications.ToPaginatedAsync(pageIndex, pageSize);
        return ServicePaginatedResult<PendingManometrVerification>.Success(result);
    }

    public async Task<ServiceResult> ProcessIncomingExcelAsync(Stream fileStream, string fileName, string sheetName, string dataRange, DeviceLocation location)
    {
        try
        {
            var verifications = _pendingVerificationsProcessor.ReadVerificationFile(fileStream, fileName, sheetName, dataRange, location);
            _logger.LogInformation("Файл {FileName} лист {Sheet} диапазоном {DataRange} содержит {Count} записей.", fileName, sheetName, dataRange, verifications.Count);
            var result = await _addCommand.ExecuteAsync(verifications);

            if (result.Error != null) return ServiceResult.Fail(result.Error);
            if (result.NewCount!.Value == 0) return ServiceResult.Success(result.Message!);

            _keeper.Signal(BackgroundEvents.GetDevicesType);

            return ServiceResult.Success(result.Message!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке файла {FileName}", fileName);
            return ServiceResult.Fail(ex.Message);
        }
    }
}
