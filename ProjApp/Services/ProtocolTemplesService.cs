using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Mapping;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class ProtocolTemplesService
{
    private readonly ProjDatabase _database;
    private readonly IMapper _mapper;
    private readonly AddProtocolTemplateCommand _addCommand;
    private readonly EventKeeper _keeper;

    public ProtocolTemplesService(ProjDatabase database,
        IMapper mapper,
        AddProtocolTemplateCommand addCommand,
        EventKeeper keeper)
    {
        _database = database;
        _mapper = mapper;
        _addCommand = addCommand;
        _keeper = keeper;
    }

    public async Task<ServicePaginatedResult<ProtocolTemplateDTO>> GetProtocolsAsync(int pageNumber, int pageSize)
    {
        var result = await _database.ProtocolTemplates
            .ProjectToType<ProtocolTemplateDTO>(_mapper.Config)
            .ToPaginatedAsync(pageNumber, pageSize);

        return ServicePaginatedResult<ProtocolTemplateDTO>.Success(result);
    }

    public async Task<ServiceResult> AddProtocolAsync(ProtocolTemplate protocol)
    {
        var result = await _addCommand.ExecuteAsync(protocol);
        if (result.Error != null) return ServiceResult.Fail(result.Error);
        if (result.DuplicateCount > 0) return ServiceResult.Fail("Протокол уже существует");
        _keeper.Signal(BackgroundEvents.NewProtocolTemplate);
        return ServiceResult.Success("Протокол добавлен");
    }

    public async Task<ServiceResult> DeleteProtocolAsync(Guid id)
    {
        var template = await _database.ProtocolTemplates.FindAsync(id);
        if (template == null) return ServiceResult.Fail("Протокол не найден");
        _database.ProtocolTemplates.Remove(template);
        await _database.SaveChangesAsync();
        return ServiceResult.Success("Протокол удален");
    }

    public async Task<ServicePaginatedResult<PossibleTemplateVerificationMethodsDTO>> GetPossibleVerificationMethodsAsync(int pageIndex, int pageSize)
    {
        var dtoList = await _database.ProtocolTemplates
            .SelectMany(template => _database.SuccessInitialVerifications
                .Where(v => v.VerificationGroup == template.VerificationGroup)
                .Where(v => v.Device!.DeviceType!.VerificationMethodId != null)
                .Select(v => v.Device!.DeviceType!.VerificationMethodId!)
                .Distinct()
                .Join(
                    _database.VerificationMethods,
                    vmId => vmId,
                    vm => vm.Id,
                    (vmId, vm) => new PossibleTemplateVerificationMethodsDTO
                    {
                        ProtocolGroup = template.ProtocolGroup,
                        VerificationMethod = vm
                    }
                )
            )
            .ToListAsync();

        var result = dtoList.ToPaginated(pageIndex, pageSize);

        return ServicePaginatedResult<PossibleTemplateVerificationMethodsDTO>.Success(result);
    }

    public async Task<ServiceResult> AddVerificationMethodAsync(Guid templateId, Guid verificationMethodId)
    {
        throw new NotImplementedException();
    }
}

public class PossibleTemplateVerificationMethodsDTO
{
    public required ProtocolGroup ProtocolGroup { get; init; }
    public required VerificationMethod VerificationMethod { get; init; }
}
