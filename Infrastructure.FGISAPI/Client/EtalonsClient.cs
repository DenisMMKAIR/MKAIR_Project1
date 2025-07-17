using Infrastructure.FGIS.Database;
using Infrastructure.FGIS.Database.Entities;
using Infrastructure.FGISAPI.RequestResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FGISAPI.Client;

internal class EtalonsClient : FGISClientBase
{
    private readonly ILogger _logger;

    public EtalonsClient(ILogger logger, HTTPQueueManager httpQueueManager, HttpClient httpClient) : base(logger, httpQueueManager, httpClient)
    {
        _logger = logger;
    }

    public async Task<ClientResponse<Etalon>> GetEtalonsAsync(
        FGISDatabase db,
        MonthResult monthResult,
        IReadOnlyList<VerificationWithEtalon> vrfs,
        bool AllDataFetched)
    {
        if (monthResult.EtalonsCollected)
        {
            _logger.LogInformation("Эталоны загружены из кэша");

            var etalonNums = vrfs
                        .SelectMany(v => v.Means.Mieta.Select(mi => mi.RegNumber))
                        .Distinct()
                        .ToArray();

            var etalons = await db.Etalons
                .Where(e => etalonNums.Contains(e.Number))
                .ToArrayAsync();

            return ClientResponse<Etalon>.Success(etalons, AllDataFetched);
        }

        IReadOnlyList<EtalonsId> etalonsIds;

        if (monthResult.EtalonsIdsCollected)
        {
            _logger.LogInformation("ID эталонов загружены из кэша");
            etalonsIds = await db.EtalonIds.Where(e => e.Date == monthResult.Date).ToArrayAsync();
        }
        else
        {
            var etalonsIdResult = await CollectEtalonsIds(db, monthResult, vrfs, AllDataFetched);
            if (etalonsIdResult.Error != null) return ClientResponse<Etalon>.Fail(etalonsIdResult.Error);
            etalonsIds = etalonsIdResult.Data!;
        }

        return await CollectEtalons(db, monthResult, etalonsIds, AllDataFetched);
    }

    private async Task<ClientResponse<EtalonsId>> CollectEtalonsIds(
        FGISDatabase db,
        MonthResult monthResult,
        IReadOnlyList<VerificationWithEtalon> verifications,
        bool allDataCollected)
    {
        _logger.LogInformation("Загрузка ID эталонов");

        IReadOnlyList<string> dbNumbers = await db.EtalonIds
            .Where(e => e.Date == monthResult.Date)
            .Select(e => e.RegNumber)
            .ToListAsync();

        _logger.LogInformation("ID эталонов в базе {Count}", dbNumbers.Count);

        IReadOnlyList<string> numsToDownload = verifications
            .SelectMany(v => v.Means.Mieta.Select(mi => mi.RegNumber))
            .Where(num => !dbNumbers.Contains(num))
            .Distinct()
            .ToList();

        _logger.LogInformation("Нужно загрузить ID эталонов {Count}", numsToDownload.Count);
        var rows = 100u;

        foreach (var etalonRegNums in numsToDownload.SplitBy(rows))
        {
            var query = $"?rows={rows}&search={string.Join("%20", etalonRegNums)}";
            var result = await GetItemListAsync<ListResponse<EtalonIdResponse>>("mieta", query);

            if (result == null)
            {
                _logger.LogError("Не удалось загрузить ID эталонов");
                return ClientResponse<EtalonsId>.Fail("Не удалось загрузить ID эталонов");
            }

            if (result.Result.Count == 0)
            {
                _logger.LogError("Не удалось получить количество ID эталонов");
                return ClientResponse<EtalonsId>.Fail("Не удалось получить количество ID эталонов");
            }

            if (result.Result.Count != etalonRegNums.Count)
            {
                _logger.LogError("Получен неполный список ID эталонов. {Count} из {TotalCount}", result.Result.Count, etalonRegNums.Count);
                return ClientResponse<EtalonsId>.Fail("Получен неполный список ID эталонов");
            }

            db.EtalonIds.AddRange(result.Result.Items.Select(e => new EtalonsId(e.Rmieta_id, e.Number, monthResult.Date)));
            await db.SaveChangesAsync();
        }

        if (allDataCollected)
        {
            monthResult.EtalonsIdsCollected = true;
            await db.SaveChangesAsync();
            _logger.LogInformation("Все ID эталонов сохранены в БД");
        }
        else
        {
            _logger.LogInformation("ID эталонов сохранены в БД");
        }

        IReadOnlyList<EtalonsId> etalonsIds = await db.EtalonIds
            .Where(e => e.Date == monthResult.Date)
            .ToArrayAsync();

        return ClientResponse<EtalonsId>.Success(etalonsIds, allDataCollected);
    }

    private async Task<ClientResponse<Etalon>> CollectEtalons(
        FGISDatabase db,
        MonthResult monthResult,
        IReadOnlyList<EtalonsId> etalonsIds,
        bool allDataCollected)
    {
        _logger.LogInformation("Загрузка эталонов");

        IReadOnlyList<string> dbEtalonNums = await db.Etalons
            .Select(e => e.Number)
            .Distinct()
            .ToListAsync();

        _logger.LogInformation("Эталонов в базе {Count}", dbEtalonNums.Count);

        IReadOnlyList<string> idsToDownload = etalonsIds
            .Where(e => !dbEtalonNums.Contains(e.RegNumber))
            .Select(e => e.Rmieta_id)
            .ToList();

        _logger.LogInformation("Нужно загрузить эталонов {Count}", idsToDownload.Count);
        var downloadedEtalonsCount = 0;
        const int chunkSize = 20;

        foreach (var chunk in idsToDownload.SplitBy(chunkSize))
        {
            var etalonsToSave = new List<Etalon>(chunkSize);

            foreach (var etalonId in chunk)
            {
                var result = await GetItemAsync<EtalonResponse>("mieta", etalonId);
                if (result == null) continue;
                etalonsToSave.Add(result.Result);
            }

            db.Etalons.AddRange(etalonsToSave);
            downloadedEtalonsCount += etalonsToSave.Count;
            await db.SaveChangesAsync();
            _logger.LogInformation("Добавлено эталонов {Count} из {TotalCount}", downloadedEtalonsCount, idsToDownload.Count);
        }

        if (downloadedEtalonsCount != idsToDownload.Count)
        {
            _logger.LogError("Получен неполный список Эталонов. {EtalonsCount} из {IdsCount}", downloadedEtalonsCount, idsToDownload.Count);
            return ClientResponse<Etalon>.Fail($"Получен неполный список Эталонов. {downloadedEtalonsCount} из {idsToDownload.Count}");
        }

        if (allDataCollected)
        {
            monthResult.EtalonsCollected = true;
            await db.SaveChangesAsync();
            _logger.LogInformation("Все эталоны сохранены в БД");
        }
        else
        {
            _logger.LogInformation("Эталоны сохранены в БД");
        }

        IReadOnlyList<Etalon> etalons = await db.Etalons
            .Join(db.EtalonIds,
                e => e.Number,
                eId => eId.RegNumber,
                (e, eId) => new { e, eId })
            .Where(dto => dto.eId.Date == monthResult.Date)
            .Select(dto => dto.e)
            .ToArrayAsync();

        return ClientResponse<Etalon>.Success(etalons, allDataCollected);
    }
}
