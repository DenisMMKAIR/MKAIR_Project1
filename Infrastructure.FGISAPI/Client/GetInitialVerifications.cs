using Infrastructure.FGIS.Database;
using Infrastructure.FGIS.Database.Entities;
using Infrastructure.FGISAPI.RequestResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;
using ProjApp.Database.SupportTypes;
using FIGSEtalon = Infrastructure.FGIS.Database.Entities.Etalon;
using FIGSDeviceType = Infrastructure.FGIS.Database.Entities.DeviceType;
using ProjectDevice = ProjApp.Database.Entities.Device;
using ProjectDeviceType = ProjApp.Database.Entities.DeviceType;
using ProjectEtalon = ProjApp.Database.Entities.Etalon;

namespace Infrastructure.FGISAPI.Client;

public partial class FGISAPIClient
{
    public async Task<(IReadOnlyList<SuccessInitialVerification>, IReadOnlyList<FailedInitialVerification>)> GetInitialVerifications(YearMonth requestDate)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FGISDatabase>();

        var monthResult = await db.MonthResults.FindAsync(requestDate);
        if (monthResult == null)
        {
            monthResult = new MonthResult(requestDate);
            await db.MonthResults.AddAsync(monthResult);
            await db.SaveChangesAsync();
        }

        if (monthResult.Done)
        {
            return await GetFromCache(db, monthResult);
        }

        _logger.LogInformation("Загрузка данных из ФГИС");
        IReadOnlyList<VerificationId> verificationIds;
        var allDataCollected = true;

        if (monthResult.VerificationIdsCollected)
        {
            _logger.LogInformation("ID поверок уже загружены");
            verificationIds = await db.VerificationIds.Where(v => v.Date == requestDate).ToArrayAsync();
        }
        else
        {
            verificationIds = await CollectVerificationIds(monthResult, db);
        }

        if (monthResult.VerificationsCollected)
        {
            _logger.LogInformation("Поверки уже загружены");
        }
        else
        {
            allDataCollected &= await CollectVerifications(monthResult, db, verificationIds);
        }

        var collectionVriId = verificationIds.Select(v => v.Vri_id).ToList();
        var verifications = await db.Verifications.Where(v => collectionVriId.Contains(v.Vri_id)).ToArrayAsync();

        if (monthResult.EtalonsIdsCollected)
        {
            _logger.LogInformation("ID эталонов уже загружены");
        }
        else
        {
            await CollectEtalonsIds(monthResult, db, verifications, allDataCollected);
        }

        var etalonsIds = await db.EtalonIds.Where(e => e.Date == requestDate).ToArrayAsync();

        if (monthResult.EtalonsCollected)
        {
            _logger.LogInformation("Эталоны уже загружены");
        }
        else
        {
            allDataCollected &= await CollectEtalons(monthResult, db, etalonsIds, allDataCollected);
        }

        var collectionNums = etalonsIds.Select(e => e.RegNumber).ToList();
        var etalons = await db.Etalons.Where(e => collectionNums.Contains(e.Number)).ToArrayAsync();

        if (monthResult.DeviceTypeIdsCollected)
        {
            _logger.LogInformation("ID типов устройств уже загружены");
        }
        else
        {
            await CollectDeviceTypeIds(monthResult, db, verifications, allDataCollected);
        }

        if (monthResult.DeviceTypesCollected)
        {
            _logger.LogInformation("Типы устройств уже загружены");
        }
        else
        {
            allDataCollected &= await CollectDeviceTypes(monthResult, db, allDataCollected);
        }

        var deviceTypes = await db.DeviceTypeIds
            .Join(db.DeviceTypes,
                  dtId => dtId.MIT_UUID,
                  dt => dt.Id,
                  (dtId, dt) => dt)
            .ToArrayAsync();

        var projectVerification = MapAllVerifications(verifications, etalons, deviceTypes);

        if (allDataCollected)
        {
            monthResult.Done = true;
            await db.SaveChangesAsync();
            _logger.LogInformation("Загрузка данных {Date} из ФГИС завершена", monthResult.Date);
        }
        else
        {
            _logger.LogError("Не удалось загрузить все данные");
        }

        return projectVerification;
    }

    private async Task<bool> CollectDeviceTypes(MonthResult monthResult, FGISDatabase db, bool allDataCollected)
    {
        _logger.LogInformation("Загрузка типов устройств");

        var dbIds = await db.DeviceTypes
            .Select(dt => dt.Number)
            .ToArrayAsync();

        _logger.LogInformation("В базе уже загружено Типов устройств {Count}", dbIds.Length);

        var idsToDownload = await db.DeviceTypeIds
            .Where(e => !dbIds.Contains(e.Number))
            .Select(e => e.MIT_UUID)
            .ToArrayAsync();

        _logger.LogInformation("Нужно загрузить Типов устройств {Count}", idsToDownload.Length);

        var downloadedDeviceTypes = new List<FIGSDeviceType>(idsToDownload.Length);
        const int chunkSize = 20;

        foreach (var chunk in idsToDownload.SplitBy(chunkSize))
        {
            var deviceTypesToSave = new List<FIGSDeviceType>(chunkSize);

            foreach (var deviceId in chunk)
            {
                var result = await GetItemAsync<DeviceTypeResponse>("mit", deviceId.ToString());
                if (result == null) continue;
                deviceTypesToSave.Add(result.ToDeviceType());
            }

            downloadedDeviceTypes.AddRange(deviceTypesToSave);
            db.DeviceTypes.AddRange(deviceTypesToSave);
            await db.SaveChangesAsync();
            _logger.LogInformation("Добавлено типов устройств {Count} из {TotalCount}", downloadedDeviceTypes.Count, idsToDownload.Length);
        }

        if (downloadedDeviceTypes.Count != idsToDownload.Length)
        {
            _logger.LogError("Получен неполный список типов устройств. {DeviceTypesCount} из {IdsCount}", downloadedDeviceTypes.Count, idsToDownload.Length);
            return false;
        }

        if (allDataCollected)
        {
            monthResult.DeviceTypesCollected = true;
            await db.SaveChangesAsync();
        }

        _logger.LogInformation("Все типы устройств сохранены в БД");
        return true;
    }

    private async Task CollectDeviceTypeIds(MonthResult monthResult, FGISDatabase db, IReadOnlyList<Verification> verifications, bool allDataCollected)
    {
        _logger.LogInformation("Загрузка ID типов устройств");
        var rows = 100u;

        var dbDeviceIds = await db.DeviceTypeIds
            .ToArrayAsync();

        _logger.LogInformation("В базе уже загружено ID типов устройств {Count}", dbDeviceIds.Length);

        var numsToDownload = verifications
            .Select(v => v.MiInfo.SingleMI.MitypeNumber)
            .Where(num => dbDeviceIds.All(dId => dId.Number != num))
            .Distinct()
            .ToArray();

        _logger.LogInformation("Нужно загрузить ID типов устройств {Count}", numsToDownload.Length);

        foreach (var deviceNums in numsToDownload.SplitBy(rows))
        {
            var query = $"?rows={rows}&search={string.Join("%20", deviceNums)}";
            var result = await GetItemListAsync<ListResponse<DeviceTypeIdResponse>>("mit", query);

            if (result == null)
            {
                throw new Exception("Не удалось загрузить ID типов устройств");
            }

            if (result.Result.Count == 0)
            {
                throw new Exception("Не удалось получить количество ID типов устройств");
            }

            if (result.Result.Count != deviceNums.Count)
            {
                throw new Exception($"Получен неполный список ID типов устройств. {result.Result.Count} из {deviceNums.Count}");
            }

            db.DeviceTypeIds.AddRange(result.Result.Items.Select(e => new DeviceTypeId(e.MIT_UUID, e.Number)));
            await db.SaveChangesAsync();
        }

        if (allDataCollected)
        {
            monthResult.DeviceTypeIdsCollected = true;
            await db.SaveChangesAsync();
        }
        _logger.LogInformation("Все ID типов устройств сохранены в БД");
    }

    private async Task<bool> CollectEtalons(MonthResult monthResult, FGISDatabase db, IReadOnlyList<EtalonsId> etalonsIds, bool allDataCollected)
    {
        _logger.LogInformation("Загрузка эталонов");

        var dbEtalonNums = await db.Etalons
            .Select(e => e.Number)
            .Distinct()
            .ToListAsync();

        _logger.LogInformation("В базе уже загружено эталонов {Count}", dbEtalonNums.Count);

        var idsToDownload = etalonsIds
            .Where(e => !dbEtalonNums.Contains(e.RegNumber))
            .Select(e => e.Rmieta_id)
            .ToList();

        _logger.LogInformation("Нужно загрузить эталонов {Count}", idsToDownload.Count);
        var downloadedEtalons = new List<FIGSEtalon>(idsToDownload.Count);
        const int chunkSize = 20;

        foreach (var chunk in idsToDownload.SplitBy(chunkSize))
        {
            var etalonsToSave = new List<FIGSEtalon>(chunkSize);

            foreach (var etalonId in chunk)
            {
                var result = await GetItemAsync<EtalonResponse>("mieta", etalonId);
                if (result == null) continue;
                etalonsToSave.Add(result.Result);
            }

            downloadedEtalons.AddRange(etalonsToSave);
            db.Etalons.AddRange(etalonsToSave);
            await db.SaveChangesAsync();
            _logger.LogInformation("Добавлено эталонов {Count} из {TotalCount}", downloadedEtalons.Count, idsToDownload.Count);
        }

        if (downloadedEtalons.Count != idsToDownload.Count)
        {
            _logger.LogError("Получен неполный список Эталонов. {EtalonsCount} из {IdsCount}", downloadedEtalons.Count, idsToDownload.Count);
            return false;
        }

        if (allDataCollected)
        {
            monthResult.EtalonsCollected = true;
            await db.SaveChangesAsync();
        }

        _logger.LogInformation("Все эталоны сохранены в БД");
        return true;
    }

    private async Task CollectEtalonsIds(MonthResult monthResult, FGISDatabase db, IReadOnlyList<Verification> verifications, bool allDataCollected)
    {
        _logger.LogInformation("Загрузка ID эталонов");
        var rows = 100u;

        var dbNumbers = await db.EtalonIds
            .Where(e => e.Date == monthResult.Date)
            .Select(e => e.RegNumber)
            .ToListAsync();

        _logger.LogInformation("В базе уже загружено ID эталонов {Count}", dbNumbers.Count);

        var numsToDownload = verifications
            .SelectMany(v => v.Means.Mieta.Select(mi => mi.RegNumber))
            .Where(num => !dbNumbers.Contains(num))
            .Distinct()
            .ToList();

        _logger.LogInformation("Нужно загрузить ID эталонов {Count}", numsToDownload.Count);

        foreach (var etalonRegNums in numsToDownload.SplitBy(rows))
        {
            var query = $"?rows={rows}&search={string.Join("%20", etalonRegNums)}";
            var result = await GetItemListAsync<ListResponse<EtalonIdResponse>>("mieta", query);

            if (result == null)
            {
                throw new Exception("Не удалось загрузить ID эталонов");
            }

            if (result.Result.Count == 0)
            {
                throw new Exception("Не удалось получить количество ID эталонов");
            }

            if (result.Result.Count != etalonRegNums.Count)
            {
                throw new Exception($"Получен неполный список ID эталонов. {result.Result.Count} из {etalonRegNums.Count}");
            }

            db.EtalonIds.AddRange(result.Result.Items.Select(e => new EtalonsId(e.Rmieta_id, e.Number, monthResult.Date)));
            await db.SaveChangesAsync();
        }

        if (allDataCollected)
        {
            monthResult.EtalonsIdsCollected = true;
            await db.SaveChangesAsync();
        }
        _logger.LogInformation("Все ID эталонов сохранены в БД");
    }

    private async Task<bool> CollectVerifications(MonthResult monthResult, FGISDatabase db, IReadOnlyList<VerificationId> verificationIds)
    {
        _logger.LogInformation("Загрузка поверок");
        const int chunkSize = 20;

        var downloadedVriIds = await db.Verifications
            .Where(v => v.VriInfo.VrfDate.Year == monthResult.Date.Year && v.VriInfo.VrfDate.Month == monthResult.Date.Month)
            .Select(v => v.Vri_id)
            .ToListAsync();

        _logger.LogInformation("В базе уже загружено поверок {Count}", downloadedVriIds.Count);

        var idsToDownload = verificationIds
            .Where(vid => !downloadedVriIds.Contains(vid.Vri_id))
            .ToList();

        _logger.LogInformation("Нужно загрузить поверок {Count}", idsToDownload.Count);
        var downloadedVerfs = new List<Verification>(idsToDownload.Count);

        foreach (var chunk in idsToDownload.SplitBy(chunkSize))
        {
            var verifsToSave = new List<Verification>(chunkSize);

            foreach (var verificationId in chunk)
            {
                var result = await GetItemAsync<VerificationResponse>("vri", verificationId.Vri_id);
                if (result == null) continue;
                verifsToSave.Add(result.Result.ToVerification(verificationId.Vri_id));
            }

            db.Verifications.AddRange(verifsToSave);
            downloadedVerfs.AddRange(verifsToSave);
            await db.SaveChangesAsync();
            _logger.LogInformation("Загружено поверок {Count} из {TotalCount}", downloadedVerfs.Count, idsToDownload.Count);
        }

        if (downloadedVerfs.Count != idsToDownload.Count)
        {
            _logger.LogError("Получен неполный список поверок. {VerfsCount} из {IdsCount}", downloadedVerfs.Count, idsToDownload.Count);
            return false;
        }

        monthResult.VerificationsCollected = true;
        await db.SaveChangesAsync();
        _logger.LogInformation("Все поверки сохранены в БД");
        return true;
    }

    private async Task<IReadOnlyList<VerificationId>> CollectVerificationIds(MonthResult monthResult, FGISDatabase db)
    {
        var fromDate = monthResult.Date.ToDateOnly().ToString("yyyy-MM-dd");
        var toDate = monthResult.Date.ToEndMonthDate().ToString("yyyy-MM-dd");
        var rows = 100;
        var start = 0;
        int? totalCount = null;
        _logger.LogInformation("Получение ID поверок со ФГИС");

        var downloadedCount = 0;
        var newIds = new List<VerificationId>();

        var dbIds = await db.VerificationIds
            .Where(v => v.Date == monthResult.Date)
            .ToArrayAsync();

        _logger.LogInformation("В базе уже загружено ID поверок {Count}", dbIds.Length);

        while (true)
        {
            var query = $"?org_title=ООО%20\"МКАИР\"&rows={rows}&start={start}&verification_date_start={fromDate}&verification_date_end={toDate}";
            var result = await GetItemListAsync<ListResponse<VerificationIdResponse>>("vri", query);

            if (result == null)
            {
                throw new Exception("Не удалось загрузить ID поверок");
            }

            if (result.Result.Count == 0)
            {
                throw new Exception("Не удалось получить количество ID поверок");
            }

            totalCount ??= result.Result.Count;
            downloadedCount += result.Result.Items.Count;

            var items = result.Result.Items
                .Where(v => dbIds.All(dbId => v.Vri_Id != dbId.Vri_id))
                .Select(v => new VerificationId(v.Vri_Id, monthResult.Date));

            newIds.AddRange(items);

            start += rows;
            if (start >= totalCount) break;
        }

        _logger.LogInformation("Получено ID поверок {Count}. Новых ID поверок {NewCount}", downloadedCount, newIds.Count);

        if (downloadedCount != totalCount)
        {
            throw new Exception($"Получен неполный список ID поверок. {downloadedCount} из {totalCount}");
        }

        db.VerificationIds.AddRange(newIds);
        monthResult.VerificationIdsCollected = true;
        await db.SaveChangesAsync();
        _logger.LogInformation("Все ID поверок сохранены в БД");

        return dbIds.Concat(newIds).ToArray();
    }

    private async Task<(IReadOnlyList<SuccessInitialVerification>, IReadOnlyList<FailedInitialVerification>)> GetFromCache(FGISDatabase db, MonthResult monthResult)
    {
        _logger.LogInformation("Данные на {Date} возвращены из кэша", monthResult.Date);

        var fgisVerification = await db.Verifications
            .Where(v => v.VriInfo.VrfDate.Year == monthResult.Date.Year && v.VriInfo.VrfDate.Month == monthResult.Date.Month)
            .ToArrayAsync();

        var etalonNums = fgisVerification
            .SelectMany(v => v.Means.Mieta.Select(mi => mi.RegNumber))
            .Distinct()
            .ToArray();

        var fgisEtalons = await db.Etalons
            .Where(e => etalonNums.Contains(e.Number))
            .ToArrayAsync();

        var deviceTypeNums = fgisVerification
            .Select(v => v.MiInfo.SingleMI.MitypeNumber)
            .ToArray();

        var fgisDeviceTypes = await db.DeviceTypes
            .Where(dt => deviceTypeNums.Contains(dt.Number))
            .ToArrayAsync();

        return MapAllVerifications(fgisVerification, fgisEtalons, fgisDeviceTypes);
    }

    private static (IReadOnlyList<SuccessInitialVerification>, IReadOnlyList<FailedInitialVerification>) MapAllVerifications(
        IReadOnlyList<Verification> fgisVerification,
        IReadOnlyList<FIGSEtalon> fgisEtalons,
        IReadOnlyList<FIGSDeviceType> fgisDeviceTypes)
    {

        var goodVrfs = fgisVerification
            .Where(v => v.VriInfo.Applicable != null)
            .ToList();

        var failedVrfs = fgisVerification
            .Where(v => v.VriInfo.Inapplicable != null)
            .ToList();

        static SuccessInitialVerification MapGood(Verification v, ProjectDeviceType deviceType, ProjectDevice device, IReadOnlyList<ProjectEtalon> etalons)
        {
            return new SuccessInitialVerification
            {
                DeviceTypeNumber = deviceType.Number,
                DeviceSerial = device.Serial,
                Owner = v.VriInfo.MiOwner,
                VerificationTypeName = v.VriInfo.DocTitle,
                VerificationDate = v.VriInfo.VrfDate,
                VerifiedUntilDate = v.VriInfo.ValidDate!.Value,
                Device = device,
                Etalons = [.. etalons],
            };
        }

        static FailedInitialVerification MapFailed(Verification v, ProjectDeviceType deviceType, ProjectDevice device, IReadOnlyList<ProjectEtalon> etalons)
        {
            return new FailedInitialVerification
            {
                DeviceTypeNumber = deviceType.Number,
                DeviceSerial = device.Serial,
                Owner = v.VriInfo.MiOwner,
                VerificationTypeName = v.VriInfo.DocTitle,
                VerificationDate = v.VriInfo.VrfDate,
                Device = device,
                Etalons = [.. etalons],
                FailedDocNumber = v.VriInfo.Inapplicable!.NoticeNum,
            };
        }

        return (MapToInitialVerifications(goodVrfs, fgisEtalons, fgisDeviceTypes, MapGood),
                MapToInitialVerifications(failedVrfs, fgisEtalons, fgisDeviceTypes, MapFailed));
    }

    private static IReadOnlyList<T> MapToInitialVerifications<T>(
        IReadOnlyList<Verification> FGISVerifications,
        IReadOnlyList<FIGSEtalon> FGISEtalons,
        IReadOnlyList<FIGSDeviceType> FGISDeviceTypes,
        Func<Verification, ProjectDeviceType, ProjectDevice, IReadOnlyList<ProjectEtalon>, T> map)
        where T : IInitialVerification
    {
        var projEtalons = FGISEtalons
            .SelectMany(FGISEtalonToProjectEtalon)
            .ToArray();

        var projDeviceTypes = FGISDeviceTypes
            .Select(FGISDeviceTypeToProjectDeviceType)
            .ToArray();

        var projVerifications = FGISVerifications
            .Select(v =>
            {
                var deviceType = projDeviceTypes.Single(dt => dt.Number == v.MiInfo.SingleMI.MitypeNumber);

                var device = new ProjectDevice { DeviceType = deviceType, DeviceTypeNumber = deviceType.Number, Serial = v.MiInfo.SingleMI.ManufactureNum, ManufacturedYear = (uint)v.MiInfo.SingleMI.ManufactureYear, Modification = v.MiInfo.SingleMI.Modification };

                var mietaNumbers = v.Means.Mieta
                    .Select(mi => mi.RegNumber)
                    .Distinct()
                    .ToList();

                var etalons = projEtalons
                    .Where(e => v.VriInfo.VrfDate >= e.Date && v.VriInfo.VrfDate <= e.ToDate)
                    .Where(e => mietaNumbers.Contains(e.Number))
                    .DistinctBy(e => e.Number)
                    .ToList();

                if (etalons.Count == 0 || etalons.Count < mietaNumbers.Count)
                {
                    throw new Exception($"Не найдены или найдены частично эталоны поверки [НомерТипа]{deviceType.Number} [СерийныйУстройства]{device.Serial} [Дата]{v.VriInfo.VrfDate}. Номер в базе фгис: {v.Vri_id}");
                }

                return map(v, deviceType, device, etalons);
            })
            .ToArray();

        return projVerifications;
    }

    private static IEnumerable<ProjectEtalon> FGISEtalonToProjectEtalon(FIGSEtalon e)
    {
        return e.CResults
            .Select(cr =>
            {
                var fullInfo = $"{e.Number}; {e.MiType_Num}; {e.MiType}; {e.MiNotation}; {e.Modification}; {e.Factory_Num}; {e.Year}; {e.RankCode}; {e.RankClass}; {e.Schematitle}; свидетельство о поверке № {cr.Result_Docnum}; действительно до {cr.Valid_Date};";

                return new ProjectEtalon
                {
                    Number = e.Number,
                    Date = DateOnly.Parse(cr.Verification_Date),
                    ToDate = DateOnly.Parse(cr.Valid_Date),
                    FullInfo = fullInfo,
                };
            });
    }

    private static ProjectDeviceType FGISDeviceTypeToProjectDeviceType(FIGSDeviceType dt)
    {
        return new ProjectDeviceType
        {
            Number = dt.Number,
            Title = dt.Title,
            Notation = string.Join("; ", dt.Notation),
            MethodUrls = dt.Meth?.Select(m => m.DocUrl).ToArray() ?? [],
            SpecUrls = dt.Spec?.Select(s => s.DocUrl).ToArray() ?? [],
            Manufacturers = dt.Manufacturer?.Select(m => m.Title).ToArray() ?? [],
        };
    }
}
