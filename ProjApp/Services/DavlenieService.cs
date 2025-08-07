using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;
using ProjApp.Mapping;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class DavlenieService
{
    private readonly ILogger<DavlenieService> _logger;
    private readonly ProjDatabase _database;

    public DavlenieService(ILogger<DavlenieService> logger, ProjDatabase database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<ServicePaginatedResult<Davlenie1VerificationDTO>> GetVerificationsAsync(int page, int pageSize, string? deviceTypeFilter, string? deviceSerialFilter, YearMonth? dateFilter, DeviceLocation? locationFilter)
    {
        var query = _database.Davlenie1Verifications.ToDTO();

        if (deviceTypeFilter != null)
        {
            query = query.Where(v => v.DeviceTypeNumber.Contains(deviceTypeFilter));
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
}
