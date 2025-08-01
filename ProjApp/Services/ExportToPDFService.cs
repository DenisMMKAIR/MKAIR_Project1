using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.InfrastructureInterfaces;
using ProjApp.ProtocolForms;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class ExportToPDFService
{
    private readonly ILogger<ExportToPDFService> _logger;
    private readonly ProjDatabase _database;
    private readonly IGetVerificationsFromExcelProcessor _excelGetVerificationsProcessor;
    private readonly PDFFilePathManager _pdfFilePathManager;
    private readonly ITemplateProcessor _templateProcessor;

    public ExportToPDFService(
        ILogger<ExportToPDFService> logger,
        ProjDatabase database,
        IGetVerificationsFromExcelProcessor excelGetVerificationsProcessor,
        PDFFilePathManager pdfFilePathManager,
        ITemplateProcessor templateProcessor)
    {
        _logger = logger;
        _database = database;
        _excelGetVerificationsProcessor = excelGetVerificationsProcessor;
        _pdfFilePathManager = pdfFilePathManager;
        _templateProcessor = templateProcessor;
    }

    public async Task<ServiceResult> ExportToPdfAsync(VerificationGroup group, IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0) return ServiceResult.Fail("Не выбрано ни одной записи");

        return group switch
        {
            VerificationGroup.Манометры => await ExportAsync<Manometr1Verification>(ids, null, cancellationToken),
            VerificationGroup.Датчики_давления => await ExportAsync<Davlenie1Verification>(ids, null, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(group), group, null),
        };
    }

    public async Task<ServiceResult> ExportAllToPdfAsync(VerificationGroup group, CancellationToken cancellationToken = default)
    {
        return group switch
        {
            VerificationGroup.Манометры => await ExportAsync<Manometr1Verification>([], null, cancellationToken),
            VerificationGroup.Датчики_давления => await ExportAsync<Davlenie1Verification>([], null, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(group), group, null),
        };
    }

    public async Task<ServiceResult> ExportByExcelToPdfAsync(
        VerificationGroup group,
        string fileName,
        Stream file,
        string sheetName,
        string dataRange,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<IVerificationBase> excelVrfs;

        try
        {
            excelVrfs = _excelGetVerificationsProcessor.GetVerificationsFromFile(file, fileName, sheetName, dataRange);
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
            .Select(v => new BaseVrfQuery
            {
                Id = v.Id,
                DeviceTypeNumber = v.DeviceTypeNumber,
                DeviceSerial = v.DeviceSerial,
                VerificationDate = v.VerificationDate
            })
            .Union(_database.Davlenie1Verifications
                .Select(v => new BaseVrfQuery
                {
                    Id = v.Id,
                    DeviceTypeNumber = v.DeviceTypeNumber,
                    DeviceSerial = v.DeviceSerial,
                    VerificationDate = v.VerificationDate
                }))
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
            // TODO: Export mechanism must handle duplicate filenames
            // We need to export files in "Current export job directory"
            // for example: "25.07.01_08.00_exportByFile{optional}_GUID"
            resultExportPDF = group switch
            {
                VerificationGroup.Манометры => await ExportAsync<Manometr1Verification>(idsToExport, fileName, cancellationToken),
                VerificationGroup.Датчики_давления => await ExportAsync<Davlenie1Verification>(idsToExport, fileName, cancellationToken),
                _ => throw new ArgumentOutOfRangeException(nameof(group), group, null),
            };
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
    
    private async Task<ServiceResult> ExportAsync<T>(
        IReadOnlyList<Guid> ids,
        string? exportDir = null,
        CancellationToken cancellationToken = default)
            where T : class, ICompleteVerification
    {
        var query = _database.Set<T>()
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
            var result = await _templateProcessor.CreatePDFAsync(vrf.ToProtocolForm(), _pdfFilePathManager.GetFilePath(vrf, exportDir), cancellationToken);
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

    private class BaseVrfQuery : IVerificationBase
    {
        public Guid Id { get; set; }
        public required string DeviceTypeNumber { get; set; }
        public required string DeviceSerial { get; set; }
        public required DateOnly VerificationDate { get; set; }
        public Device? Device { get; set; }
        public VerificationMethod? VerificationMethod { get; set; }
        public ICollection<Etalon>? Etalons { get; set; }
    }
}