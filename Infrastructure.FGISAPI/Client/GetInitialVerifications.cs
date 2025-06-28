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
    public async Task<IReadOnlyList<InitialVerification>> GetInitialVerifications(YearMonth requestDate)
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

        if (monthResult.Done) return await GetFromCache(db, monthResult);

        IReadOnlyList<VerificationId> verificationIds;

        _logger.LogInformation("Загрузка данных из ФГИС");

        if (monthResult.VerificationIdsCollected)
        {
            _logger.LogInformation("ID поверок уже загружены");
            verificationIds = await db.VerificationIds.Where(v => v.Date == requestDate).ToListAsync();
        }
        else
        {
            _logger.LogInformation("Загрузка ID поверок");
            verificationIds = await CollectVerificationIds(monthResult, db);
        }

        IReadOnlyList<Verification> verifications;

        if (monthResult.VerificationsCollected)
        {
            _logger.LogInformation("Поверки уже загружены");
            var ids = verificationIds.Select(v => v.Vri_id).ToList();
            verifications = await db.Verifications.Where(v => ids.Contains(v.Vri_id)).ToListAsync();
        }
        else
        {
            _logger.LogInformation("Загрузка поверок");
            verifications = await CollectVerifications(monthResult, db, verificationIds);
        }

        IReadOnlyList<EtalonsId> etalonsIds;

        if (monthResult.EtalonsIdsCollected)
        {
            _logger.LogInformation("ID эталонов уже загружены");
            etalonsIds = await db.EtalonIds.Where(e => e.Date == requestDate).ToListAsync();
        }
        else
        {
            _logger.LogInformation("Загрузка ID эталонов");
            etalonsIds = await CollectEtalonsIds(monthResult, db, verifications);
        }

        IReadOnlyList<FIGSEtalon> etalons;

        if (monthResult.EtalonsCollected)
        {
            _logger.LogInformation("Эталоны уже загружены");
            var ids = etalonsIds.Select(e => e.RegNumber).ToList();
            etalons = await db.Etalons.Where(e => ids.Contains(e.Number)).ToListAsync();
        }
        else
        {
            _logger.LogInformation("Загрузка эталонов");
            etalons = await CollectEtalons(monthResult, db, etalonsIds);
        }

        var projectVerification = MapToInitialVerifications(verifications, etalons);

        monthResult.Done = true;
        await db.SaveChangesAsync();

        return projectVerification;
    }

    private async Task<IReadOnlyList<FIGSEtalon>> CollectEtalons(MonthResult monthResult, FGISDatabase db, IReadOnlyList<EtalonsId> etalonsIds)
    {
        var etalons = new List<FIGSEtalon>(etalonsIds.Count);
        const int chunkSize = 20;

        foreach (var chunk in etalonsIds.SplitBy(chunkSize))
        {
            var etalonsToSave = new List<FIGSEtalon>(chunkSize);

            foreach (var etalonId in chunk)
            {
                var result = await GetItemAsync<EtalonResponse>("mieta", etalonId.Rmieta_id);
                etalonsToSave.Add(result.Result);
            }

            etalons.AddRange(etalonsToSave);
            db.Etalons.AddRange(etalonsToSave);
            await db.SaveChangesAsync();
        }

        if (etalonsIds.Count != etalons.Count)
        {
            throw new Exception($"Получен неполный список Эталонов. {etalons.Count} из {etalonsIds.Count}");
        }

        monthResult.EtalonsCollected = true;
        await db.SaveChangesAsync();

        return etalons;
    }

    private async Task<IReadOnlyList<EtalonsId>> CollectEtalonsIds(MonthResult monthResult, FGISDatabase db, IReadOnlyList<Verification> verifications)
    {
        var rows = 100u;
        var etalonsIds = new List<EtalonsId>();

        foreach (var etalonRegNums in verifications.SelectMany(v => v.Means.Mieta.Select(mi => mi.RegNumber)).Distinct().SplitBy(rows))
        {
            var query = $"?rows={rows}&search={string.Join("%20", etalonRegNums)}";
            var result = await GetItemListAsync<ListResponse<EtalonIdResponse>>("mieta", query);

            if (result.Result.Count == 0)
            {
                throw new Exception("Не удалось получить количество ID эталонов");
            }

            etalonsIds.AddRange(result.Result.Items.Select(e => new EtalonsId(e.Rmieta_id, e.Number, monthResult.Date)));

            if (result.Result.Count != etalonRegNums.Count)
            {
                throw new Exception($"Получен неполный список ID эталонов. {result.Result.Count} из {etalonRegNums.Count}");
            }
        }

        db.EtalonIds.AddRange(etalonsIds);
        monthResult.EtalonsIdsCollected = true;
        await db.SaveChangesAsync();

        return etalonsIds;
    }

    private async Task<IReadOnlyList<Verification>> CollectVerifications(MonthResult monthResult, FGISDatabase db, IReadOnlyList<VerificationId> verificationIds)
    {
        var verfs = new List<Verification>(verificationIds.Count);
        const int chunkSize = 20;

        foreach (var chunk in verificationIds.SplitBy(chunkSize))
        {
            var verifsToSave = new List<Verification>(chunkSize);

            foreach (var verificationId in chunk)
            {
                var result = await GetItemAsync<VerificationResponse>("vri", verificationId.Vri_id);
                verifsToSave.Add(result.Result.ToVerification(verificationId.Vri_id));
            }

            verfs.AddRange(verifsToSave);
            db.Verifications.AddRange(verifsToSave);
            await db.SaveChangesAsync();
        }

        if (verificationIds.Count != verfs.Count)
        {
            throw new Exception($"Получен неполный список поверок. {verfs.Count} из {verificationIds.Count}");
        }

        monthResult.VerificationsCollected = true;
        await db.SaveChangesAsync();

        return verfs;
    }

    private async Task<IReadOnlyList<VerificationId>> CollectVerificationIds(MonthResult monthResult, FGISDatabase db)
    {
        var fromDate = monthResult.Date.ToDateOnly().ToString("yyyy-MM-dd");
        var toDate = monthResult.Date.ToEndMonthDate().ToString("yyyy-MM-dd");
        var rows = 100;
        var verificationIds = new List<VerificationId>();
        var start = 0;
        int? count = null;

        while (true)
        {
            var query = $"?org_title=ООО%20\"МКАИР\"&rows={rows}&start={start}&verification_date_start={fromDate}&verification_date_end={toDate}";

            var result = await GetItemListAsync<ListResponse<VerificationIdResponse>>("vri", query);

            if (result.Result.Count == 0)
            {
                throw new Exception("Не удалось получить количество ID поверок");
            }

            verificationIds.AddRange(result.Result.Items.Select(v => new VerificationId(v.Vri_Id, monthResult.Date)));
            count ??= result.Result.Count;

            start += rows;
            if (start >= count) break;
        }

        if (verificationIds.Count != count)
        {
            throw new Exception($"Получен неполный список ID поверок. {verificationIds.Count} из {count}");
        }

        db.VerificationIds.AddRange(verificationIds);
        monthResult.VerificationIdsCollected = true;
        await db.SaveChangesAsync();

        return verificationIds;
    }

    private async Task<IReadOnlyList<InitialVerification>> GetFromCache(FGISDatabase db, MonthResult monthResult)
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

        return MapToInitialVerifications(fgisVerification, fgisEtalons);
    }

    private static InitialVerification[] MapToInitialVerifications(IReadOnlyList<Verification> FGISVerifications, IReadOnlyList<FIGSEtalon> FGISEtalons)
    {
        var projEtalons = FGISEtalons
            .SelectMany(FGISEtalonToProjectEtalon)
            .ToList();

        var projVerifications = FGISVerifications
            .Select(v =>
            {
                var deviceType = new ProjectDeviceType { Number = v.MiInfo.SingleMI.MitypeNumber, Title = v.MiInfo.SingleMI.MitypeTitle, Notation = v.MiInfo.SingleMI.MitypeType };

                var device = new ProjectDevice { DeviceType = deviceType, Serial = v.MiInfo.SingleMI.ManufactureNum, ManufacturedYear = (uint)v.MiInfo.SingleMI.ManufactureYear };

                var mietaNumbers = v.Means.Mieta
                    .Select(mi => mi.RegNumber)
                    .Distinct()
                    .ToList();

                var etalons = projEtalons
                    .Where(e => v.VriInfo.VrfDate >= e.Date && v.VriInfo.VrfDate <= e.ToDate)
                    .Where(e => mietaNumbers.Contains(e.Number))
                    .DistinctBy(e => e.Number)
                    .ToList();

                if (etalons.Count < mietaNumbers.Count)
                {
                    throw new Exception($"Не найдены или найдены частично эталоны поверки [НомерТипа]{deviceType.Number} [СерийныйУстройства]{device.Serial} [Дата]{v.VriInfo.VrfDate}. Номер в базе фгис: {v.Vri_id}");
                }

                return new InitialVerification
                {
                    DeviceTypeNumber = deviceType.Number,
                    DeviceSerial = device.Serial,
                    Owner = v.VriInfo.MiOwner,
                    AdditionalInfo = v.Info.Additional_Info,
                    VerificationTypeName = v.VriInfo.DocTitle,
                    VerificationDate = v.VriInfo.VrfDate,
                    VerifiedUntilDate = v.VriInfo.ValidDate,
                    Device = device,
                    Etalons = etalons,
                };
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
