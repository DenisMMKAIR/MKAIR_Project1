using Mapster;
using MapsterMapper;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Database.SupportTypes;
using ProjApp.Mapping;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class InitialVerificationService
{
    private readonly ProjDatabase _database;
    private readonly IMapper _mapper;
    private readonly AddInitialVerificationCommand<InitialVerification> _addInitialVerificationCommand;

    public InitialVerificationService(
        ProjDatabase database,
        IMapper mapper,
        AddInitialVerificationCommand<InitialVerification> addInitialVerificationCommand)
    {
        _database = database;
        _mapper = mapper;
        _addInitialVerificationCommand = addInitialVerificationCommand;
    }

    public async Task<ServicePaginatedResult<InitialVerificationDto>> GetInitialVerifications(int page, int pageSize, YearMonth? yearMonthFilter)
    {
        var query = _database.InitialVerifications.AsQueryable();

        if (yearMonthFilter != null)
        {
            query = query.Where(iv => iv.VerificationDate.Year == yearMonthFilter.Value.Year && iv.VerificationDate.Month == yearMonthFilter.Value.Month);
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
}
