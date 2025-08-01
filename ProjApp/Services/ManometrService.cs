using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;
using ProjApp.Mapping;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class ManometrService
{
    private readonly ILogger<ManometrService> _logger;
    private readonly ProjDatabase _database;

    public ManometrService(
        ILogger<ManometrService> logger,
        ProjDatabase database)
    {
        _logger = logger;
        _database = database;
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
}

public class ChangeManometr1VerificationRequest { }
