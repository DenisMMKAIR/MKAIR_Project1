using Infrastructure.FGIS.Database;
using Infrastructure.FGIS.Database.Entities;
using Infrastructure.FGISAPI.RequestResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FGISAPI.Client;

internal class VerificationsClient : FGISClientBase
{
    private readonly ILogger _logger;

    public VerificationsClient(ILogger logger, HTTPQueueManager httpQueueManager, HttpClient httpClient) : base(logger, httpQueueManager, httpClient)
    {
        _logger = logger;
    }

    public async Task<ClientResponse<VerificationWithEtalon>> GetVerificationsAsync(FGISDatabase db, MonthResult monthResult)
    {
        if (monthResult.VerificationsCollected)
        {
            _logger.LogInformation("Поверки загружены из кэша");
            var vrfs = await db.VerificationsWithEtalon
                .Where(v => v.VriInfo.VrfDate.Year == monthResult.Date.Year &&
                            v.VriInfo.VrfDate.Month == monthResult.Date.Month)
                .ToArrayAsync();
            return ClientResponse<VerificationWithEtalon>.Success(vrfs, true);
        }

        IReadOnlyList<VerificationId> vrfIds;

        if (monthResult.VerificationIdsCollected)
        {
            _logger.LogInformation("ID поверок загружены из кэша");
            vrfIds = await db.VerificationIds.Where(v => v.Date == monthResult.Date).ToArrayAsync();
        }
        else
        {
            var vrfIdsResult = await CollectVerificationIdsAsync(db, monthResult);
            if (vrfIdsResult.Error != null) return ClientResponse<VerificationWithEtalon>.Fail(vrfIdsResult.Error);
            vrfIds = vrfIdsResult.Data!;
        }

        return await CollectVerificationsAsync(db, monthResult, vrfIds);
    }

    private async Task<ClientResponse<VerificationId>> CollectVerificationIdsAsync(FGISDatabase db, MonthResult monthResult)
    {
        _logger.LogInformation("Получение ID поверок со ФГИС");

        var fromDate = monthResult.Date.ToDateOnly().ToString("yyyy-MM-dd");
        var toDate = monthResult.Date.ToEndMonthDate().ToString("yyyy-MM-dd");
        var rows = 100;
        var start = 0;

        int? totalCount = null;

        var dbIds = await db.VerificationIds
            .Where(v => v.Date == monthResult.Date)
            .ToArrayAsync();

        _logger.LogInformation("В базе загружено ID поверок {Count}", dbIds.Length);
        var newIds = new List<VerificationId>();

        while (true)
        {
            var query = $"?org_title=ООО%20\"МКАИР\"&rows={rows}&start={start}&verification_date_start={fromDate}&verification_date_end={toDate}";
            var result = await GetItemListAsync<ListResponse<VerificationIdResponse>>("vri", query);

            if (result == null)
            {
                _logger.LogError("Не удалось загрузить ID поверок");
                return ClientResponse<VerificationId>.Fail("Не удалось загрузить ID поверок");
            }

            if (result.Result.Count == 0)
            {
                _logger.LogError("Не удалось получить количество ID поверок");
                return ClientResponse<VerificationId>.Fail("Не удалось получить количество ID поверок");
            }

            totalCount ??= result.Result.Count;

            var items = result.Result.Items
                .Where(v => dbIds.All(dbId => v.Vri_Id != dbId.Vri_id))
                .Select(v => new VerificationId(v.Vri_Id, monthResult.Date));

            newIds.AddRange(items);

            start += rows;
            if (start >= totalCount) break;
        }

        _logger.LogInformation("Загружено ID поверок {Count}", newIds.Count);

        if (newIds.Count != totalCount)
        {
            _logger.LogError("Получен неполный список ID поверок. {NewCount} из {TotalCount}", newIds.Count, totalCount);
            return ClientResponse<VerificationId>.Fail($"Получен неполный список ID поверок. {newIds.Count} из {totalCount}");
        }

        db.VerificationIds.AddRange(newIds);
        monthResult.VerificationIdsCollected = true;
        await db.SaveChangesAsync();
        _logger.LogInformation("Все ID поверок сохранены в БД");

        return ClientResponse<VerificationId>.Success(newIds, true);
    }

    private async Task<ClientResponse<VerificationWithEtalon>> CollectVerificationsAsync(
        FGISDatabase db,
        MonthResult monthResult,
        IReadOnlyList<VerificationId> verificationIds)
    {
        _logger.LogInformation("Загрузка поверок");
        const int chunkSize = 20;

        IReadOnlyList<string> dbVriIds = await db.VerificationsWithEtalon
            .Select(v => new { v.Vri_id, v.VriInfo.VrfDate })
            .Union(db.VerificationsWithtSes
                .Select(v => new { v.Vri_id, v.VriInfo.VrfDate }))
            .Where(v => v.VrfDate.Year == monthResult.Date.Year && v.VrfDate.Month == monthResult.Date.Month)
            .Select(v => v.Vri_id)
            .ToArrayAsync();

        _logger.LogInformation("В базе уже загружено поверок {Count}", dbVriIds.Count);

        IReadOnlyList<VerificationId> idsToDownload = verificationIds
            .Where(vid => !dbVriIds.Contains(vid.Vri_id))
            .ToArray();

        _logger.LogInformation("Нужно загрузить поверок {Count}", idsToDownload.Count);
        var downloadedEtaVrfs = 0;
        var downloadedSesVrfs = 0;
        var downloadedMutualVrfs = 0;

        foreach (var chunk in idsToDownload.SplitBy(chunkSize))
        {
            var etaVrfsToSave = new List<VerificationWithEtalon>(chunkSize);
            var sesVrfsTosave = new List<VerificationWithSes>(chunkSize);

            foreach (var verificationId in chunk)
            {
                var result = await GetItemAsync<VerificationResponse>("vri", verificationId.Vri_id);
                if (result == null) continue;
                switch (result.Result.Means)
                {
                    case { Mieta.Count: > 0 }:
                        etaVrfsToSave.Add(result.Result.ToEtaVerification(verificationId.Vri_id));
                        break;
                    case { Ses.Count: > 0 }:
                        sesVrfsTosave.Add(result.Result.ToSesVerification(verificationId.Vri_id));
                        break;
                    default:
                        continue;
                }
            }

            db.VerificationsWithEtalon.AddRange(etaVrfsToSave);
            db.VerificationsWithtSes.AddRange(sesVrfsTosave);
            downloadedEtaVrfs += etaVrfsToSave.Count;
            downloadedSesVrfs += sesVrfsTosave.Count;
            downloadedMutualVrfs += etaVrfsToSave.Count + sesVrfsTosave.Count;
            await db.SaveChangesAsync();
            _logger.LogInformation("Загружено поверок с эталонами {EtaCount}. Загружено поверок с образцами {SesCount}. {MutualCount} из {TotalCount}", downloadedEtaVrfs, downloadedSesVrfs, downloadedMutualVrfs, idsToDownload.Count);
        }

        var resultVrfs = await db.VerificationsWithEtalon
            .Where(v => v.VriInfo.VrfDate.Year == monthResult.Date.Year && v.VriInfo.VrfDate.Month == monthResult.Date.Month)
            .ToArrayAsync();

        if (downloadedMutualVrfs != idsToDownload.Count)
        {
            _logger.LogError("Получен неполный список поверок. {VerfsCount} из {IdsCount}", downloadedMutualVrfs, idsToDownload.Count);
            return ClientResponse<VerificationWithEtalon>.Success(resultVrfs, false);
        }

        monthResult.VerificationsCollected = true;
        await db.SaveChangesAsync();
        _logger.LogInformation("Все поверки сохранены в БД");

        return ClientResponse<VerificationWithEtalon>.Success(resultVrfs, true);
    }
}
