using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
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

    public async Task<ServicePaginatedResult<Manometr1VerificationDto>> GetVerificationsAsync(int page, int pageSize)
    {
        var result = await _database.Manometr1Verifications
            .ProjectToType<Manometr1VerificationDto>(_mapper.Config)
            .ToPaginatedAsync(page, pageSize);

        return ServicePaginatedResult<Manometr1VerificationDto>.Success(result);
    }

    public Task<ServiceResult> ChangeVerificationAsync(ChangeManometr1VerificationRequest dto)
        => throw new NotImplementedException();

    public Task<ServiceResult> DeleteVerificationAsync(Guid id)
        => throw new NotImplementedException();

    public async Task<ServiceResult> ExportToPdfAsync(IReadOnlyList<Guid> ids)
    {
        var query = _database.Manometr1Verifications
            .Where(x => ids.Contains(x.Id));

        var successCount = 0;
        var failedCount = 0;

        foreach (var vrf in query)
        {
            var result = await _templateProcessor.CreatePDFAsync(vrf);
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
        config.NewConfig<Manometr1Verification, Manometr1VerificationDto>();
    }
}

public class ChangeManometr1VerificationRequest { }
