using Infrastructure.FGIS.Database;
using Infrastructure.FGIS.Database.Entities;
using Infrastructure.FGISAPI.RequestResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;
using ProjApp.Database.SupportTypes;
using FIGSEtalon = Infrastructure.FGIS.Database.Entities.Etalon;
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
            verificationIds = await db.VerificationIds.Where(v => v.Date == requestDate).ToListAsync();
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
        var verifications = await db.Verifications.Where(v => collectionVriId.Contains(v.Vri_id)).ToListAsync();

        if (monthResult.EtalonsIdsCollected)
        {
            _logger.LogInformation("ID эталонов уже загружены");
        }
        else
        {
            await CollectEtalonsIds(monthResult, db, verifications, allDataCollected);
        }

        var etalonsIds = await db.EtalonIds.Where(e => e.Date == requestDate).ToListAsync();

        if (monthResult.EtalonsCollected)
        {
            _logger.LogInformation("Эталоны уже загружены");
        }
        else
        {
            allDataCollected &= await CollectEtalons(monthResult, db, etalonsIds, allDataCollected);
        }

        var collectionNums = etalonsIds.Select(e => e.RegNumber).ToList();
        var etalons = await db.Etalons.Where(e => collectionNums.Contains(e.Number)).ToListAsync();

        var projectVerification = MapAllVerifications(verifications, etalons);

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

    private async Task<bool> CollectEtalons(MonthResult monthResult, FGISDatabase db, IReadOnlyList<EtalonsId> etalonsIds, bool allDataCollected)
    {
        _logger.LogInformation("Загрузка эталонов");

        var downloadedEtalonNums = await db.Etalons
            .Select(e => e.Number)
            .Distinct()
            .ToListAsync();

        _logger.LogInformation("В базе уже загружено эталонов {Count}", downloadedEtalonNums.Count);

        var idsToDownload = etalonsIds
            .Where(e => !downloadedEtalonNums.Contains(e.RegNumber))
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
            .ToListAsync();

        var etalonNums = fgisVerification
            .SelectMany(v => v.Means.Mieta.Select(mi => mi.RegNumber))
            .Distinct()
            .ToList();

        var fgisEtalons = await db.Etalons
            .Where(e => etalonNums.Contains(e.Number))
            .ToListAsync();

        return MapAllVerifications(fgisVerification, fgisEtalons);
    }

    private static (IReadOnlyList<SuccessInitialVerification>, IReadOnlyList<FailedInitialVerification>) MapAllVerifications(
        IReadOnlyList<Verification> fgisVerification,
        IReadOnlyList<FIGSEtalon> fgisEtalons)
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

        return (MapToInitialVerifications(goodVrfs, fgisEtalons, MapGood),
                MapToInitialVerifications(failedVrfs, fgisEtalons, MapFailed));
    }

    private static IReadOnlyList<T> MapToInitialVerifications<T>(
        IReadOnlyList<Verification> FGISVerifications,
        IReadOnlyList<FIGSEtalon> FGISEtalons,
        Func<Verification, ProjectDeviceType, ProjectDevice, IReadOnlyList<ProjectEtalon>, T> map)
        where T : IInitialVerification
    {
        var projEtalons = FGISEtalons
            .SelectMany(FGISEtalonToProjectEtalon)
            .ToList();

        var projVerifications = FGISVerifications
            .Select(v =>
            {
                var deviceType = new ProjectDeviceType { Number = v.MiInfo.SingleMI.MitypeNumber, Title = v.MiInfo.SingleMI.MitypeTitle, Notation = v.MiInfo.SingleMI.MitypeType };

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
}
