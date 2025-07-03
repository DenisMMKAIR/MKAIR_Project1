using Mapster;
using MapsterMapper;
using ProjApp.Database;
using ProjApp.Mapping;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class InitialVerificationService
{
    private readonly ProjDatabase _database;
    private readonly IMapper _mapper;

    public InitialVerificationService(ProjDatabase database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public async Task<ServicePaginatedResult<InitialVerificationDto>> GetInitialVerifications(int page, int pageSize)
    {
        var result = await _database.InitialVerifications
            .ProjectToType<InitialVerificationDto>(_mapper.Config)
            .ToPaginatedAsync(page, pageSize);
        return ServicePaginatedResult<InitialVerificationDto>.Success(result);
    }

    public async Task<ServicePaginatedResult<FailedInitialVerificationDto>> GetFailedInitialVerifications(int page, int pageSize)
    {
        var result = await _database.InitialVerificationsFailed
            .ProjectToType<FailedInitialVerificationDto>(_mapper.Config)
            .ToPaginatedAsync(page, pageSize);
        return ServicePaginatedResult<FailedInitialVerificationDto>.Success(result);
    }
}
