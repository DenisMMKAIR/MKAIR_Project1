using Infrastructure.FGIS.Database;
using Infrastructure.FGIS.Database.Entities;
using Infrastructure.FGISAPI.RequestResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FGISAPI.Client;

internal class DeviceTypeClient : FGISClientBase
{
    private readonly ILogger _logger;

    public DeviceTypeClient(ILogger logger, HTTPQueueManager httpQueueManager, HttpClient httpClient) : base(logger, httpQueueManager, httpClient)
    {
        _logger = logger;
    }

    public async Task<ClientResponse<DeviceType>> GetDeviceTypesAsync(
        FGISDatabase db,
        MonthResult monthResult,
        IReadOnlyList<VerificationWithEtalon> vrfs,
        bool allDataCollected)
    {
        if (monthResult.DeviceTypesCollected)
        {
            _logger.LogInformation("Типы устройств загружены из кэша");
            var deviceTypes = await GetDeviceTypesFromDBAsync(db, monthResult);
            return ClientResponse<DeviceType>.Success(deviceTypes, allDataCollected);
        }

        IReadOnlyList<DeviceTypeId> deviceTypeIds;

        if (monthResult.DeviceTypeIdsCollected)
        {
            _logger.LogInformation("ID типов устройств загружены из кэша");
            deviceTypeIds = await GetDeviceTypeIdsFromDBAsync(db, monthResult);
        }
        else
        {
            var deviceTypeIdResult = await CollectDeviceTypeIds(db, monthResult, vrfs, allDataCollected);
            if (deviceTypeIdResult.Error != null) return ClientResponse<DeviceType>.Fail(deviceTypeIdResult.Error);
            deviceTypeIds = deviceTypeIdResult.Data!;
        }

        return await CollectDeviceTypes(db, monthResult, deviceTypeIds, allDataCollected);
    }

    private async Task<ClientResponse<DeviceTypeId>> CollectDeviceTypeIds(
        FGISDatabase db,
        MonthResult monthResult,
        IReadOnlyList<VerificationWithEtalon> verifications,
        bool allDataCollected)
    {
        _logger.LogInformation("Загрузка ID типов устройств");

        var dbDeviceIds = await GetDeviceTypeIdsFromDBAsync(db, monthResult);

        _logger.LogInformation("ID типов устройств в базе {Count}", dbDeviceIds.Count);

        IReadOnlyList<string> numsToDownload = verifications
            .Select(v => v.MiInfo.SingleMI.MitypeNumber)
            .Where(num => dbDeviceIds.All(dId => dId.Number != num))
            .Distinct()
            .ToArray();

        _logger.LogInformation("Нужно загрузить ID типов устройств {Count}", numsToDownload.Count);
        const uint rows = 100;

        foreach (var deviceNums in numsToDownload.SplitBy(rows))
        {
            var query = $"?rows={rows}&search={string.Join("%20", deviceNums)}";
            var result = await GetItemListAsync<ListResponse<DeviceTypeIdResponse>>("mit", query);

            if (result == null)
            {
                _logger.LogError("Не удалось загрузить ID типов устройств");
                return ClientResponse<DeviceTypeId>.Fail("Не удалось загрузить ID типов устройств");
            }

            if (result.Result.Count == 0)
            {
                _logger.LogError("Не удалось получить количество ID типов устройств");
                return ClientResponse<DeviceTypeId>.Fail("Не удалось получить количество ID типов устройств");
            }

            if (result.Result.Count != deviceNums.Count)
            {
                _logger.LogError("Получен неполный список ID типов устройств. {DeviceTypesCount} из {IdsCount}", result.Result.Count, deviceNums.Count);
                return ClientResponse<DeviceTypeId>.Fail($"Получен неполный список ID типов устройств. {result.Result.Count} из {deviceNums.Count}");
            }

            db.DeviceTypeIds.AddRange(result.Result.Items.Select(e => new DeviceTypeId(e.MIT_UUID, e.Number)));
            await db.SaveChangesAsync();
        }

        if (allDataCollected)
        {
            monthResult.DeviceTypeIdsCollected = true;
            await db.SaveChangesAsync();
            _logger.LogInformation("Все ID типов устройств сохранены в БД");
        }
        else
        {
            _logger.LogInformation("ID типов устройств сохранены в БД");
        }

        dbDeviceIds = await GetDeviceTypeIdsFromDBAsync(db, monthResult);

        return ClientResponse<DeviceTypeId>.Success(dbDeviceIds, allDataCollected);
    }

    private async Task<ClientResponse<DeviceType>> CollectDeviceTypes(
        FGISDatabase db,
        MonthResult monthResult,
        IReadOnlyList<DeviceTypeId> deviceTypeIds,
        bool allDataCollected)
    {
        _logger.LogInformation("Загрузка типов устройств");

        IReadOnlyList<string> dbNumbers = (await GetDeviceTypesFromDBAsync(db, monthResult))
            .Select(dt => dt.Number)
            .ToArray();

        _logger.LogInformation("Типов устройств в базе {Count}", dbNumbers.Count);

        IReadOnlyList<Guid> idsToDownload = deviceTypeIds
            .Where(dtId => !dbNumbers.Contains(dtId.Number))
            .Select(e => e.MIT_UUID)
            .ToArray();

        _logger.LogInformation("Нужно загрузить Типов устройств {Count}", idsToDownload.Count);

        var downloadedDeviceTypesCount = 0;
        const int chunkSize = 20;

        foreach (var chunk in idsToDownload.SplitBy(chunkSize))
        {
            var deviceTypesToSave = new List<DeviceType>(chunkSize);

            foreach (var deviceId in chunk)
            {
                var result = await GetItemAsync<DeviceTypeResponse>("mit", deviceId.ToString());
                if (result == null) continue;
                deviceTypesToSave.Add(result.ToDeviceType());
            }

            db.DeviceTypes.AddRange(deviceTypesToSave);
            downloadedDeviceTypesCount += deviceTypesToSave.Count;
            await db.SaveChangesAsync();
            _logger.LogInformation("Добавлено типов устройств {Count} из {TotalCount}", downloadedDeviceTypesCount, idsToDownload.Count);
        }

        if (downloadedDeviceTypesCount != idsToDownload.Count)
        {
            _logger.LogError("Получен неполный список типов устройств. {DeviceTypesCount} из {IdsCount}", downloadedDeviceTypesCount, idsToDownload.Count);
            return ClientResponse<DeviceType>.Fail($"Получен неполный список типов устройств. {downloadedDeviceTypesCount} из {idsToDownload.Count}");
        }

        if (allDataCollected)
        {
            monthResult.DeviceTypesCollected = true;
            await db.SaveChangesAsync();
            _logger.LogInformation("Все типы устройств сохранены в БД");
        }
        else
        {
            _logger.LogInformation("Типы устройств сохранены в БД");
        }

        var dbDeviceTypes = await GetDeviceTypesFromDBAsync(db, monthResult);
        return ClientResponse<DeviceType>.Success(dbDeviceTypes, allDataCollected);
    }

    private static async Task<IReadOnlyList<DeviceTypeId>> GetDeviceTypeIdsFromDBAsync(FGISDatabase db, MonthResult monthResult)
    {
        return await db.DeviceTypeIds
            .Join(
                db.VerificationsWithEtalon
                .Select(v => new { v.MiInfo.SingleMI.MitypeNumber, v.VriInfo.VrfDate })
                .Union(db.VerificationsWithtSes
                    .Select(v => new { v.MiInfo.SingleMI.MitypeNumber, v.VriInfo.VrfDate })),
                dtId => dtId.Number,
                vrf => vrf.MitypeNumber,
                (dtId, vrf) => new { dtId, vrf })
            .Where(dto => dto.vrf.VrfDate.Year == monthResult.Date.Year &&
                            dto.vrf.VrfDate.Month == monthResult.Date.Month)
            .Select(dto => dto.dtId)
            .Distinct()
            .ToArrayAsync();
    }

    private static async Task<IReadOnlyList<DeviceType>> GetDeviceTypesFromDBAsync(FGISDatabase db, MonthResult monthResult)
    {
        return await db.DeviceTypes
            .Join(
                db.VerificationsWithEtalon.Select(v => new { v.MiInfo.SingleMI.MitypeNumber, v.VriInfo.VrfDate })
                .Union(db.VerificationsWithtSes.Select(v => new { v.MiInfo.SingleMI.MitypeNumber, v.VriInfo.VrfDate })),
                dt => dt.Number,
                vrf => vrf.MitypeNumber,
                (dt, vrf) => new { dt, vrf })
            .Where(dto => dto.vrf.VrfDate.Year == monthResult.Date.Year &&
                        dto.vrf.VrfDate.Month == monthResult.Date.Month)
            .Select(dto => dto.dt)
            .Distinct()
            .ToArrayAsync();
    }
}
