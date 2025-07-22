using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class ManometrService
{
    private readonly ILogger<ManometrService> _logger;
    private readonly ProjDatabase _database;
    private readonly IMapper _mapper;
    private readonly ITemplateProcessor _templateProcessor;

    public ManometrService(ILogger<ManometrService> logger, ProjDatabase database, IMapper mapper, ITemplateProcessor templateProcessor)
    {
        _logger = logger;
        _database = database;
        _mapper = mapper;
        _templateProcessor = templateProcessor;
    }

    public async Task<ServicePaginatedResult<Manometr1VerificationDto>> GetVerificationsAsync(int page, int pageSize, string? deviceTypeFilter, string? deviceSerialFilter, YearMonth? dateFilter, DeviceLocation? locationFilter)
    {
        var query = _database.Manometr1Verifications
            .ProjectToType<Manometr1VerificationDto>(_mapper.Config);

        if (deviceTypeFilter != null)
        {
            query = query.Where(dto => dto.DeviceTypeName.Contains(deviceTypeFilter));
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
        var vrfs = await _database.Manometr1Verifications.Where(x => ids.Contains(x.Id)).ToArrayAsync(cancellationToken.Value);
        if (vrfs.Length == 0) return ServiceResult.Fail("Поверки не найдены");
        _database.Manometr1Verifications.RemoveRange(vrfs);
        await _database.SaveChangesAsync();
        return ServiceResult.Success("Поверки удалены");
    }

    public async Task<ServiceResult> ExportToPdfAsync(IReadOnlyList<Guid> ids, CancellationToken? cancellationToken = null)
    {
        if (ids.Count == 0) return ServiceResult.Fail("Не выбрано ни одной записи");
        return await ExportAsync(ids, cancellationToken);
    }

    public async Task<ServiceResult> ExportAllToPdfAsync(CancellationToken cancellationToken)
    {
        return await ExportAsync([], cancellationToken);
    }

    private async Task<ServiceResult> ExportAsync(IReadOnlyList<Guid> ids, CancellationToken? cancellationToken = null)
    {
        var query = _database.Manometr1Verifications
            .Include(v => v.Device!)
                .ThenInclude(d => d.DeviceType!)
            .Include(v => v.Etalons!)
            .AsQueryable();

        if (ids.Count > 0)
        {
            query = query.Where(x => ids.Contains(x.Id));
        }

        var successCount = 0;
        var failedCount = 0;

        foreach (var vrf in query)
        {
            var result = await _templateProcessor.CreatePDFAsync(vrf, cancellationToken);
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

public class Manometr1VerificationDto : IRegister
{
    public required Guid Id { get; set; }
    public required string ProtocolNumber { get; set; }
    public required string DeviceTypeName { get; set; }
    public required string DeviceModification { get; set; }
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required uint ManufactureYear { get; set; }
    public required string Owner { get; set; }
    public required ulong OwnerINN { get; set; }
    public required string VerificationsInfo { get; set; }
    public required string EtalonsInfo { get; set; }
    public required double Temperature { get; set; }
    public required double Humidity { get; set; }
    public required string Pressure { get; set; }
    public required string VerificationVisualCheckup { get; set; }
    public required string VerificationResultCheckup { get; set; }
    public required string VerificationAccuracyCheckup { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required string Worker { get; set; }

    // Support values
    public required VerificationGroup VerificationGroup { get; set; }
    public required DeviceLocation Location { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }

    // Table values
    public required double MeasurementMin { get; set; }
    public required double MeasurementMax { get; set; }
    public required string MeasurementUnit { get; set; }
    public required double ValidError { get; set; }

    public required IReadOnlyList<IReadOnlyList<double>> DeviceValues { get; set; }
    public required IReadOnlyList<IReadOnlyList<double>> EtalonValues { get; set; }
    public required IReadOnlyList<IReadOnlyList<double>> ActualError { get; set; }
    public required IReadOnlyList<double> ActualVariation { get; set; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Manometr1Verification, Manometr1VerificationDto>()
            .Map(dest => dest.EtalonsInfo, src => string.Join(';', src.Etalons!.Select(e => e.Number)));
    }
}

public class ChangeManometr1VerificationRequest { }
