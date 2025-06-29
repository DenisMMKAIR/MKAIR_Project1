using Microsoft.EntityFrameworkCore;
using ProjApp.Database;
using ProjApp.Database.Entities;

namespace ProjApp.Services;

public class InitialVerificationService
{
    private readonly ProjDatabase _database;

    public InitialVerificationService(ProjDatabase database)
    {
        _database = database;
    }

    public async Task<ServicePaginatedResult<InitialVerification>> GetInitialVerifications(int page, int pageSize)
    {
        var result = await _database.InitialVerifications
            .Include(iv => iv.Device)
            .ThenInclude(d => d!.DeviceType)
            .Include(iv => iv.Etalons)
            .ToPaginatedAsync(page, pageSize);
        return ServicePaginatedResult<InitialVerification>.Success(result);
    }
}
