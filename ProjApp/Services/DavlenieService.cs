using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Mapping;
using ProjApp.ProtocolForms;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class DavlenieService
{
    private readonly ILogger<DavlenieService> _logger;
    private readonly ProjDatabase _database;
    private readonly ITemplateProcessor _templateProcessor;
    private readonly PDFFilePathManager _pdfFilePathManager;

    public DavlenieService(ILogger<DavlenieService> logger, ProjDatabase database, ITemplateProcessor templateProcessor, PDFFilePathManager pdfFilePathManager)
    {
        _logger = logger;
        _database = database;
        _templateProcessor = templateProcessor;
        _pdfFilePathManager = pdfFilePathManager;
    }

    public async Task<ServicePaginatedResult<Davlenie1VerificationDTO>> GetVerificationsAsync(int page, int pageSize, string? deviceTypeFilter, string? deviceSerialFilter, YearMonth? dateFilter, DeviceLocation? locationFilter)
    {
        var query = _database.Davlenie1Verifications.ToDTO();

        if (deviceTypeFilter != null)
        {
            query = query.Where(v => v.DeviceTypeName.Contains(deviceTypeFilter));
        }

        if (deviceSerialFilter != null)
        {
            query = query.Where(v => v.DeviceSerial.Contains(deviceSerialFilter));
        }

        if (dateFilter != null)
        {
            query = query.Where(v =>
                v.VerificationDate.Year == dateFilter.Value.Year &&
                v.VerificationDate.Month == dateFilter.Value.Month);
        }

        if (locationFilter != null)
        {
            query = query.Where(v => v.Location == locationFilter);
        }

        var result = await query.ToPaginatedAsync(page, pageSize);

        return ServicePaginatedResult<Davlenie1VerificationDTO>.Success(result);
    }

    public async Task<ServiceResult> DeleteVerificationAsync(IReadOnlyList<Guid> ids, CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;

        var vrfs = await _database.Davlenie1Verifications
            .Where(x => ids.Contains(x.Id))
            .ToArrayAsync(cancellationToken.Value);

        if (vrfs.Length == 0) return ServiceResult.Fail("Поверки не найдены");

        _database.Davlenie1Verifications.RemoveRange(vrfs);
        await _database.SaveChangesAsync();

        return ServiceResult.Success("Поверки удалены");
    }

    public async Task<ServiceResult> ExportToPdfAsync(IReadOnlyList<Guid> ids, CancellationToken? cancellationToken = null)
    {
        if (ids.Count == 0) return ServiceResult.Fail("Не выбрано ни одной записи");
        return await ExportAsync(ids, cancellationToken);
    }

    public async Task<ServiceResult> ExportAllToPdfAsync(CancellationToken? cancellationToken = null)
    {
        return await ExportAsync([], cancellationToken);
    }

    private async Task<ServiceResult> ExportAsync(IReadOnlyList<Guid> ids, CancellationToken? cancellationToken = null)
    {
        var query = _database.Davlenie1Verifications
            .Include(v => v.Device!)
                .ThenInclude(d => d.DeviceType!)
            .Include(v => v.Etalons!)
            .Include(v => v.VerificationMethod!)
            .Include(v => v.Owner)
            .AsNoTracking();

        if (ids.Count > 0)
        {
            query = query.Where(x => ids.Contains(x.Id));
        }

        var successCount = 0;
        var failedCount = 0;

        var vrfs = await query.ToArrayAsync();

        foreach (var vrf in vrfs)
        {
            var form = vrf.ToDavlenieForm();
            var result = await _templateProcessor.CreatePDFAsync(form, _pdfFilePathManager.GetFilePath(vrf), cancellationToken);
            if (result.Error != null)
            {
                _logger.LogError("{Error}", result.Error);
                failedCount++;
                continue;
            }
            successCount++;
        }

        return ServiceResult.Success($"Экспортировано {successCount}, ошибки экспорта {failedCount}");
    }
}
