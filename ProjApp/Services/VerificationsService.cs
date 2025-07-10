using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Mapping;
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
            query = query.Where(v => v.DeviceTypeNumber == deviceTypeNumberFilter);
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
            .OrderBy(v => v.DeviceTypeNumber)
            .ProjectToType<SuccessInitialVerificationDto>(_mapper.Config)
            .ToPaginatedAsync(page, pageSize);

        return ServicePaginatedResult<SuccessInitialVerificationDto>.Success(result);
    }

    public async Task<ServicePaginatedResult<SuccessVerificationDto>> GetVerifications(
        int page,
        int pageSize,
        string? deviceTypeNumberFilter,
        YearMonth? yearMonthFilter,
        string? typeInfoFilter,
        DeviceLocation? locationFilter)
    {
        var query = _database.SuccessVerifications.AsQueryable();

        if (deviceTypeNumberFilter != null)
        {
            query = query.Where(v => v.DeviceTypeNumber == deviceTypeNumberFilter);
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
            .ProjectToType<SuccessVerificationDto>(_mapper.Config)
            .ToPaginatedAsync(page, pageSize);

        return ServicePaginatedResult<SuccessVerificationDto>.Success(result);
    }

    public async Task<ServiceResult> AddInitialVerification(SuccessInitialVerification iv)
    {
        var result = await _addInitialVerificationCommand.ExecuteAsync(iv);
        if (result.Error != null) return ServiceResult.Fail(result.Error);
        if (result.DuplicateCount > 0) return ServiceResult.Fail("Поверка уже существует");
        return ServiceResult.Success("Поверка добавлена");
    }

    public async Task<ServiceResult> SetValues(MemoryStream file, SetValuesRequest request)
    {
        var result = await SetValuesBaseAsync(file, request, true);
        _eventKeeper.Signal(BackgroundEvents.AddedValuesInitialVerification);
        return result;
    }

    public async Task<ServiceResult> SetVerificationNum(MemoryStream file, string sheetName, string dataRange)
    {
        var request = new SetValuesRequest { DataRange = dataRange, SheetName = sheetName, VerificationTypeNum = true, Location = DeviceLocation.АнтипинскийНПЗ };
        var result = await SetValuesBaseAsync(file, request, false);
        _eventKeeper.Signal(BackgroundEvents.AddedValuesInitialVerification);
        return result;
    }

    private async Task<ServiceResult> SetValuesBaseAsync(MemoryStream file, SetValuesRequest request, bool setLocation)
    {
        IReadOnlyList<IInitialVerification> resultIvs;

        try
        {
            resultIvs = _ivAddValuesProcessor.SetFromExcelFile(file, request);
        }
        catch (Exception e)
        {
            _logger.LogError("{Error}", e.Message);
            return ServiceResult.Fail(e.Message);
        }

        var uniqComparer = new VerificationUniqComparer();

        var dbIvs = _database.SuccessInitialVerifications
            .AsEnumerable()
            .Where(iv => resultIvs.Contains(iv, uniqComparer))
            .ToArray();

        var setAny = false;

        foreach (var dbIv in dbIvs)
        {
            var resultIv = resultIvs.First(iv => uniqComparer.Equals(iv, dbIv));

            if (setLocation && dbIv.Location != request.Location)
            {
                dbIv.Location = request.Location;
                setAny = true;
            }
            if (request.VerificationTypeNum is true && dbIv.ProtocolNumber != resultIv.ProtocolNumber)
            {
                dbIv.ProtocolNumber = resultIv.ProtocolNumber;
                setAny = true;
            }
            if (request.Worker is true && dbIv.Worker != resultIv.Worker)
            {
                dbIv.Worker = resultIv.Worker;
                setAny = true;
            }
            if (request.Pressure is true && dbIv.Pressure != resultIv.Pressure)
            {
                dbIv.Pressure = resultIv.Pressure;
                setAny = true;
            }
            if (request.Temperature is true && dbIv.Temperature != resultIv.Temperature)
            {
                dbIv.Temperature = resultIv.Temperature;
                setAny = true;
            }
            if (request.Humidity is true && dbIv.Humidity != resultIv.Humidity)
            {
                dbIv.Humidity = resultIv.Humidity;
                setAny = true;
            }
            if (request.MeasurementRange is true)
            {
                var key = "min";
                if (dbIv.AdditionalInfo[key] != resultIv.AdditionalInfo[key])
                {
                    dbIv.AdditionalInfo[key] = resultIv.AdditionalInfo[key];
                    setAny = true;
                }
                key = "max";
                if (dbIv.AdditionalInfo[key] != resultIv.AdditionalInfo[key])
                {
                    dbIv.AdditionalInfo[key] = resultIv.AdditionalInfo[key];
                    setAny = true;
                }
                key = "unit";
                if (dbIv.AdditionalInfo[key] != resultIv.AdditionalInfo[key])
                {
                    dbIv.AdditionalInfo[key] = resultIv.AdditionalInfo[key];
                    setAny = true;
                }
            }
            if (request.Accuracy is true)
            {
                var key = "accuracy";
                if (dbIv.AdditionalInfo[key] != resultIv.AdditionalInfo[key])
                {
                    dbIv.AdditionalInfo[key] = resultIv.AdditionalInfo[key];
                    setAny = true;
                }
            }
        }

        if (!setAny) return ServiceResult.Success("Нет изменений");

        using var transaction = _database.Database.BeginTransaction();

        try
        {
            await _database.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult.Fail(e.Message);
        }

        return ServiceResult.Success("Данные добавлены");
    }
}

public class SetValuesRequest
{
    public required string SheetName { get; set; }
    public required string DataRange { get; set; }
    public required DeviceLocation Location { get; init; }
    public bool? VerificationTypeNum { get; init; }
    public bool? Worker { get; init; }
    public bool? Pressure { get; init; }
    public bool? Temperature { get; init; }
    public bool? Humidity { get; init; }
    public bool? MeasurementRange { get; set; }
    public bool? Accuracy { get; set; }
}
