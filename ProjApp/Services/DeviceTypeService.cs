using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class DeviceTypeService
{
    private readonly ILogger<DeviceTypeService> _logger;
    private readonly ProjDatabase _database;
    private readonly AddDeviceTypeCommand _addCommand;

    public DeviceTypeService(ILogger<DeviceTypeService> logger, ProjDatabase database, AddDeviceTypeCommand addCommand)
    {
        _logger = logger;
        _database = database;
        _addCommand = addCommand;
    }

    public async Task<ServicePaginatedResult<DeviceType>> GetPaginatedAsync(int pageIndex, int pageSize)
    {
        var paginatedResponse = await _database.DeviceTypes.ToPaginatedAsync(pageIndex, pageSize);
        return ServicePaginatedResult<DeviceType>.Success(paginatedResponse);
    }

    public async Task<ServiceResult> AddDeviceTypeAsync(DeviceType deviceType)
    {
        var result = await _addCommand.ExecuteAsync(deviceType);
        if (result.Error != null) return ServiceResult.Fail(result.Error);
        if (result.NewCount!.Value == 0) return ServiceResult.Fail("Тип устройства уже существует");
        return ServiceResult.Success($"Тип устройства {deviceType.Number} успешно добавлен");
    }
}
