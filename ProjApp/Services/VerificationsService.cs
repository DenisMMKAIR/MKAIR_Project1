using System.Collections.Immutable;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Mapping;
using ProjApp.Normalizers.VerificationMethod;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class VerificationsService
{
    private readonly ProjDatabase _database;
    private readonly IMapper _mapper;
    private readonly AddInitialVerificationCommand<SuccessInitialVerification> _addInitialVerificationCommand;
    private readonly IIVSetValuesProcessor _ivAddValuesProcessor;
    private readonly ILogger<VerificationsService> _logger;
    private readonly EventKeeper _eventKeeper;

    public VerificationsService(
        ProjDatabase database,
        IMapper mapper,
        AddInitialVerificationCommand<SuccessInitialVerification> addInitialVerificationCommand,
        IIVSetValuesProcessor ivAddValuesProcessor,
        ILogger<VerificationsService> logger,
        EventKeeper eventKeeper)
    {
        _database = database;
        _mapper = mapper;
        _addInitialVerificationCommand = addInitialVerificationCommand;
        _ivAddValuesProcessor = ivAddValuesProcessor;
        _logger = logger;
        _eventKeeper = eventKeeper;
    }

    public async Task<ServicePaginatedResult<SuccessInitialVerificationDto>> GetInitialVerifications(
        int page,
        int pageSize,
        string? deviceTypeNumberFilter,
        YearMonth? yearMonthFilter,
        string? typeInfoFilter,
        DeviceLocation? locationFilter)
    {
        var query = _database.SuccessInitialVerifications.AsQueryable();

        if (deviceTypeNumberFilter != null)
        {
            query = query.Where(v => v.DeviceTypeNumber.Contains(deviceTypeNumberFilter));
        }

        if (yearMonthFilter != null)
        {
            query = query.Where(v => v.VerificationDate.Year == yearMonthFilter.Value.Year &&
                                      v.VerificationDate.Month == yearMonthFilter.Value.Month);
        }

        if (typeInfoFilter != null)
        {
            query = query.Where(v => v.Device!.DeviceType!.Title.ToUpper().Contains(typeInfoFilter.ToUpper()));
        }

        if (locationFilter != null)
        {
            query = query.Where(v => v.Location == locationFilter);
        }

        var result = await query
            .OrderBy(v => v.VerificationDate)
            .ThenBy(v => v.DeviceTypeNumber)
            .ProjectToType<SuccessInitialVerificationDto>(_mapper.Config)
            .ToPaginatedAsync(page, pageSize);

        return ServicePaginatedResult<SuccessInitialVerificationDto>.Success(result);
    }

    public async Task<ServiceResult> AddInitialVerification(SuccessInitialVerification iv)
    {
        var result = await _addInitialVerificationCommand.ExecuteAsync([iv]);
        if (result.Error != null) return ServiceResult.Fail(result.Error);
        if (result.DuplicateCount > 0) return ServiceResult.Fail("Поверка уже существует");
        return ServiceResult.Success("Поверка добавлена");
    }

    public async Task<ServiceResult> SetValues(MemoryStream file, SetValuesRequest request)
    {
        var result = await SetValuesBaseAsync(file, request, true, true);
        return result;
    }

    public async Task<ServiceResult> SetVerificationNum(MemoryStream file, string sheetName, string dataRange)
    {
        var request = new SetValuesRequest { DataRange = dataRange, SheetName = sheetName, VerificationTypeNum = true, Location = DeviceLocation.АнтипинскийНПЗ, Group = VerificationGroup.Манометры };
        var result = await SetValuesBaseAsync(file, request, false, false);
        return result;
    }

    private async Task<ServiceResult> SetValuesBaseAsync(MemoryStream file, SetValuesRequest request, bool setLocation, bool setGroup)
    {
        HashSet<IInitialVerification> excelVrfs;
        var initialHasDuplicatesMsg = string.Empty;

        try
        {
            var excelVrfsInitial = _ivAddValuesProcessor.SetFromExcelFile(file, request);

            if (excelVrfsInitial.Count == 0)
            {
                return ServiceResult.Fail("Не найдены записи в файле");
            }

            excelVrfs = new HashSet<IInitialVerification>(excelVrfsInitial, VerificationUniqComparer.Instance);

            if (excelVrfsInitial.Count != excelVrfs.Count)
            {
                var dups = excelVrfsInitial
                    .GroupBy(v => v, VerificationUniqComparer.Instance)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                var dupsString = dups
                    .Select(v => $"{v.DeviceTypeNumber} {v.DeviceSerial} {v.VerificationDate}")
                    .Aggregate((sum, cur) => $"{sum}, {cur}");

                initialHasDuplicatesMsg = $"Дубликаты в запрошенных данных: {dupsString}";
            }
        }
        catch (Exception e)
        {
            _logger.LogError("{Error}", e.Message);
            return ServiceResult.Fail(e.Message);
        }

        var typeNumbers = excelVrfs.Select(v => v.DeviceTypeNumber).Distinct().ToArray();
        var serialNumbers = excelVrfs.Select(v => v.DeviceSerial).Distinct().ToArray();
        var dates = excelVrfs.Select(v => v.VerificationDate).Distinct().ToArray();

        var dbIvs = _database.SuccessInitialVerifications
            .Where(v => typeNumbers.Contains(v.DeviceTypeNumber) &&
                        serialNumbers.Contains(v.DeviceSerial) &&
                        dates.Contains(v.VerificationDate))
            .ToArray();

        if (dbIvs.Length == 0)
        {
            var dbHasNoDataMsg = "В базе нет данных поверок";
            if (initialHasDuplicatesMsg.Length > 0) dbHasNoDataMsg += $". {initialHasDuplicatesMsg}";
            return ServiceResult.Fail(dbHasNoDataMsg);
        }

        var changedCount = 0;

        foreach (var dbIv in dbIvs)
        {
            var setAny = false;
            if (!excelVrfs.TryGetValue(dbIv, out var excelVrf)) continue;

            if (setLocation && dbIv.Location != request.Location)
            {
                dbIv.Location = request.Location;
                setAny = true;
            }
            if (setGroup && dbIv.VerificationGroup != request.Group)
            {
                dbIv.VerificationGroup = request.Group;
                setAny = true;
            }
            if (request.VerificationTypeNum is true && dbIv.ProtocolNumber != excelVrf.ProtocolNumber)
            {
                dbIv.ProtocolNumber = excelVrf.ProtocolNumber;
                setAny = true;
            }
            if (request.Worker is true && dbIv.Worker != excelVrf.Worker)
            {
                dbIv.Worker = excelVrf.Worker;
                setAny = true;
            }
            if (request.Pressure is true && dbIv.Pressure != excelVrf.Pressure)
            {
                dbIv.Pressure = excelVrf.Pressure;
                setAny = true;
            }
            if (request.Temperature is true && dbIv.Temperature != excelVrf.Temperature)
            {
                dbIv.Temperature = excelVrf.Temperature;
                setAny = true;
            }
            if (request.Humidity is true && dbIv.Humidity != excelVrf.Humidity)
            {
                dbIv.Humidity = excelVrf.Humidity;
                setAny = true;
            }
            if (request.MeasurementRange is true)
            {
                if (dbIv.MeasurementMin != excelVrf.MeasurementMin)
                {
                    dbIv.MeasurementMin = excelVrf.MeasurementMin;
                    setAny = true;
                }
                if (dbIv.MeasurementMax != excelVrf.MeasurementMax)
                {
                    dbIv.MeasurementMax = excelVrf.MeasurementMax;
                    setAny = true;
                }
                if (dbIv.MeasurementUnit != excelVrf.MeasurementUnit)
                {
                    dbIv.MeasurementUnit = excelVrf.MeasurementUnit;
                    setAny = true;
                }
            }
            if (request.Accuracy is true && dbIv.Accuracy != excelVrf.Accuracy)
            {
                dbIv.Accuracy = excelVrf.Accuracy;
                setAny = true;
            }

            if (setAny) changedCount++;
        }

        if (changedCount == 0)
        {
            var hasNoChangesMsg = "Нет изменений";
            if (initialHasDuplicatesMsg.Length > 0) hasNoChangesMsg += $". {initialHasDuplicatesMsg}";
            return ServiceResult.Success(hasNoChangesMsg);
        }

        try
        {
            await _database.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return ServiceResult.Fail(e.Message);
        }

        _eventKeeper.Signal(BackgroundEvents.AddedValuesInitialVerification);


        var msg = "Данные добавлены";

        if (initialHasDuplicatesMsg.Length > 0)
        {
            msg += $". {initialHasDuplicatesMsg}";
        }

        var protocolNumsOnly = setGroup == false;

        if (!protocolNumsOnly)
        {
            var notFoundVrfs = excelVrfs
                .Except(dbIvs, VerificationUniqComparer.Instance)
                .ToArray();

            if (notFoundVrfs.Length > 0)
            {
                var notFoundMsg = notFoundVrfs
                    .Select(v => $"{v.DeviceTypeNumber} {v.DeviceSerial} {v.VerificationDate}")
                    .Aggregate((sum, cur) => $"{sum}, {cur}");

                msg += $". Не найдены поверки: {notFoundMsg}";
            }
        }

        return ServiceResult.Success(msg);
    }

    [Obsolete("Method used in Background service only")]
    public async Task<ServiceResult> AddVerificationMethodsAsync(IReadOnlyList<IInitialVerification> verifications)
    {
        var norm = VerificationMethodAliasComparerNormalizer.Instance.Normalize;
        bool methodHasAlias(VerificationMethod m, string a) => m.Aliases.Select(norm).Contains(norm(a));
        bool methodHasAliases(VerificationMethod m, ImmutableSortedSet<string> a) => m.Aliases.Select(norm).Any(a.Contains);

        var normAliases = verifications
            .Select(v => norm(v.VerificationTypeName))
            .ToImmutableSortedSet();

        var vms = _database.VerificationMethods
            // .Include(m => m.SuccessInitialVerifications)
            // .Include(m => m.FailedInitialVerifications)
            .AsEnumerable()
            .Where(m => methodHasAliases(m, normAliases))
            .ToArray();

        if (vms.Length == 0) return ServiceResult.Fail($"Не найдены методики поверок");

        foreach (var v in verifications)
        {
            var m = vms.FirstOrDefault(m => methodHasAlias(m, v.VerificationTypeName));
            if (m == null) continue;
            v.VerificationMethod = m;
        }

        await _database.SaveChangesAsync();
        return ServiceResult.Success("Методики поверок добавлены");
    }
}

public class SetValuesRequest
{
    public required string SheetName { get; set; }
    public required string DataRange { get; set; }
    public required DeviceLocation Location { get; init; }
    public required VerificationGroup Group { get; init; }
    public bool? VerificationTypeNum { get; init; }
    public bool? Worker { get; init; }
    public bool? Pressure { get; init; }
    public bool? Temperature { get; init; }
    public bool? Humidity { get; init; }
    public bool? MeasurementRange { get; set; }
    public bool? Accuracy { get; set; }
}
