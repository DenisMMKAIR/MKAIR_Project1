using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Mapping;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class InitialVerificationService
{
    private readonly ProjDatabase _database;
    private readonly IMapper _mapper;
    private readonly AddInitialVerificationCommand<InitialVerification> _addInitialVerificationCommand;
    private readonly IIVSetValuesProcessor _ivAddValuesProcessor;
    private readonly ILogger<InitialVerificationService> _logger;

    public InitialVerificationService(
        ProjDatabase database,
        IMapper mapper,
        AddInitialVerificationCommand<InitialVerification> addInitialVerificationCommand,
        IIVSetValuesProcessor ivAddValuesProcessor,
        ILogger<InitialVerificationService> logger)
    {
        _database = database;
        _mapper = mapper;
        _addInitialVerificationCommand = addInitialVerificationCommand;
        _ivAddValuesProcessor = ivAddValuesProcessor;
        _logger = logger;
    }

    public async Task<ServicePaginatedResult<InitialVerificationDto>> GetInitialVerifications(
        int page,
        int pageSize,
        YearMonth? yearMonthFilter,
        string? typeInfoFilter,
        DeviceLocation? locationFilter)
    {
        var query = _database.InitialVerifications.AsQueryable();

        if (yearMonthFilter != null)
        {
            query = query.Where(iv => iv.VerificationDate.Year == yearMonthFilter.Value.Year && iv.VerificationDate.Month == yearMonthFilter.Value.Month);
        }

        if (typeInfoFilter != null)
        {
            query = query.Where(iv => iv.Device!.DeviceType!.Title.Contains(typeInfoFilter));
        }

        if (locationFilter != null)
        {
            query = query.Where(iv => iv.Location == locationFilter);
        }

        var result = await query
            .ProjectToType<InitialVerificationDto>(_mapper.Config)
            .ToPaginatedAsync(page, pageSize);
        return ServicePaginatedResult<InitialVerificationDto>.Success(result);
    }

    public async Task<ServiceResult> AddInitialVerification(InitialVerification iv)
    {
        var result = await _addInitialVerificationCommand.ExecuteAsync(iv);
        if (result.Error != null) return ServiceResult.Fail(result.Error);
        if (result.DuplicateCount > 0) return ServiceResult.Fail("Поверка уже существует");
        return ServiceResult.Success("Поверка добавлена");
    }

    public async Task<ServiceResult> SetValues(MemoryStream file, SetValuesRequest request)
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

        var uniqComparer = new InitialVerificationUniqComparer<IInitialVerification>();

        var dbIvs = _database.InitialVerifications
            .AsEnumerable()
            .Where(iv => resultIvs.Contains(iv, uniqComparer))
            .ToArray();

        using var transaction = _database.Database.BeginTransaction();

        foreach (var dbIv in dbIvs)
        {
            var resultIv = resultIvs.First(iv => uniqComparer.Equals(iv, dbIv));
            dbIv.Location = request.Location;

            if (request.VerificationTypeNum is true) dbIv.VerificationTypeNum = resultIv.VerificationTypeNum;
            if (request.Worker is true) dbIv.Worker = resultIv.Worker;
            if (request.AdditionalInfo is true) dbIv.AdditionalInfo = resultIv.AdditionalInfo;
            if (request.Pressure is true) dbIv.Pressure = resultIv.Pressure;
            if (request.Temperature is true) dbIv.Temperature = resultIv.Temperature;
            if (request.Humidity is true) dbIv.Humidity = resultIv.Humidity;
        }

        throw new NotImplementedException();

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
    public bool? AdditionalInfo { get; init; }
    public bool? Pressure { get; init; }
    public bool? Temperature { get; init; }
    public bool? Humidity { get; init; }
}
