using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Mapping;
using ProjApp.ProtocolForms;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class ManometrService
{
    private readonly ILogger<ManometrService> _logger;
    private readonly ProjDatabase _database;
    private readonly ITemplateProcessor _templateProcessor;
    private readonly PDFFilePathManager _pdfFilePathManager;
    private readonly IGetVerificationsFromExcelProcessor _excelGetVerificationsProcessor;

    public ManometrService(
        ILogger<ManometrService> logger,
        ProjDatabase database,
        ITemplateProcessor templateProcessor,
        PDFFilePathManager pdfFilePathManager,
        IGetVerificationsFromExcelProcessor excelGetVerificationsProcessor)
    {
        _logger = logger;
        _database = database;
        _templateProcessor = templateProcessor;
        _pdfFilePathManager = pdfFilePathManager;
        _excelGetVerificationsProcessor = excelGetVerificationsProcessor;
    }

    public async Task<ServicePaginatedResult<Manometr1VerificationDto>> GetVerificationsAsync(int page, int pageSize, string? deviceTypeFilter, string? deviceSerialFilter, YearMonth? dateFilter, DeviceLocation? locationFilter)
    {
        var query = _database.Manometr1Verifications.ToDto();

        if (deviceTypeFilter != null)
        {
            query = query.Where(v => v.DeviceTypeName.Contains(deviceTypeFilter));
        }

        if (deviceSerialFilter != null)
        {
            query = query.Where(dto => dto.DeviceSerial.Contains(deviceSerialFilter));
        }

        if (dateFilter != null)
        {
            query = query.Where(dto =>
                dto.VerificationDate.Year == dateFilter.Value.Year &&
                dto.VerificationDate.Month == dateFilter.Value.Month);
        }

        if (locationFilter != null)
        {
            query = query.Where(dto => dto.Location == locationFilter);
        }

        var result = await query
            .ToPaginatedAsync(page, pageSize);

        return ServicePaginatedResult<Manometr1VerificationDto>.Success(result);
    }

    public Task<ServiceResult> ChangeVerificationAsync(ChangeManometr1VerificationRequest dto)
        => throw new NotImplementedException();

    public async Task<ServiceResult> DeleteVerificationAsync(IReadOnlyList<Guid> ids, CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;

        var vrfs = await _database.Manometr1Verifications
            .Where(x => ids.Contains(x.Id))
            .ToArrayAsync(cancellationToken.Value);

        if (vrfs.Length == 0) return ServiceResult.Fail("Поверки не найдены");

        _database.Manometr1Verifications.RemoveRange(vrfs);
        await _database.SaveChangesAsync();

        return ServiceResult.Success("Поверки удалены");
    }

    public async Task<ServiceResult> ExportToPdfAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0) return ServiceResult.Fail("Не выбрано ни одной записи");
        return await ExportAsync(ids, null, cancellationToken);
    }

    public async Task<ServiceResult> ExportAllToPdfAsync(CancellationToken cancellationToken)
    {
        return await ExportAsync([], null, cancellationToken);
    }

    public async Task<ServiceResult> ExportToPdfAsync(
        string fileName,
        MemoryStream excelFile,
        string sheetName,
        string dataRange,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<IVerificationBase> excelVrfs;

        try
        {
            excelVrfs = _excelGetVerificationsProcessor.GetVerificationsFromFile(excelFile, fileName, sheetName, dataRange);
        }
        catch (Exception e)
        {
            return ServiceResult.Fail(e.Message);
        }

        if (excelVrfs.Count == 0) return ServiceResult.Fail("Не найдены записи в файле для экспорта");

        var typeNumbers = excelVrfs.Select(x => x.DeviceTypeNumber).Distinct().ToArray();
        var serials = excelVrfs.Select(x => x.DeviceSerial).Distinct().ToArray();
        var dates = excelVrfs.Select(x => x.VerificationDate).Distinct().ToArray();

        var dbVrfs = await _database.Manometr1Verifications
            .Where(v => typeNumbers.Contains(v.DeviceTypeNumber) &&
                        serials.Contains(v.DeviceSerial) &&
                        dates.Contains(v.VerificationDate))
            .ToArrayAsync(cancellationToken);

        if (dbVrfs.Length == 0) return ServiceResult.Fail("Не найдены поверки для экспорта");

        var idsToExport = dbVrfs
            .GroupJoin(excelVrfs,
                       dbV => new { dbV.DeviceTypeNumber, dbV.DeviceSerial, dbV.VerificationDate },
                       exV => new { exV.DeviceTypeNumber, exV.DeviceSerial, exV.VerificationDate },
                       (dbV, exV) => new { dbV.Id, Exists = exV.FirstOrDefault() is not null })
            .Where(dto => dto.Exists)
            .Select(dto => dto.Id)
            .ToArray();

        fileName = Path.GetFileNameWithoutExtension(fileName);
        Span<char> charBuffer = stackalloc char[fileName.Length];

        for (int i = 0; i < fileName.Length; i++)
        {
            char c = fileName[i];
            charBuffer[i] = Array.IndexOf(Path.GetInvalidFileNameChars(), c) >= 0 ? '_' : c;
        }

        fileName = new string(charBuffer);
        ServiceResult? resultExportPDF;

        try
        {
            resultExportPDF = await ExportAsync(idsToExport, fileName, cancellationToken);
        }
        catch (Exception e)
        {
            return ServiceResult.Fail(e.Message);
        }

        if (resultExportPDF.Error != null) return resultExportPDF;

        var notFoundVrfs = excelVrfs.Except(dbVrfs, new VerificationUniqComparer())
            .Select(v => $"{v.DeviceTypeNumber} {v.DeviceSerial} {v.VerificationDate}")
            .ToArray();

        var msg = resultExportPDF.Message!;

        if (notFoundVrfs.Length > 0)
        {
            var notFoundVrfsString = string.Join(" | ", notFoundVrfs);
            msg = $"{msg}. Не найдены поверки {notFoundVrfsString}";
        }

        return ServiceResult.Success(msg);
    }

    private async Task<ServiceResult> ExportAsync(IReadOnlyList<Guid> ids, string? extraDir = null, CancellationToken cancellationToken = default)
    {
        var query = _database.Manometr1Verifications
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

        var vrfs = await query.ToArrayAsync(cancellationToken);

        foreach (var vrf in vrfs)
        {
            var form = vrf.ToManometrForm();
            var result = await _templateProcessor.CreatePDFAsync(form, _pdfFilePathManager.GetFilePath(vrf, extraDir), cancellationToken);
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

public class ChangeManometr1VerificationRequest { }
