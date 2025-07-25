using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;
using ProjApp.InfrastructureInterfaces;
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

    public Task<ServicePaginatedResult<object>> GetVerificationsAsync(int page, int pageSize, string? deviceTypeFilter, string? deviceSerialFilter, YearMonth? dateFilter, DeviceLocation? locationFilter)
    {
        throw new NotImplementedException();
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

    private Task<ServiceResult> ExportAsync(IReadOnlyList<Guid> ids, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }
}
