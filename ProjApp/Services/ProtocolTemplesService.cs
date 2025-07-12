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
        var template = await _database.ProtocolTemplates
            .Include(t => t.VerificationMethods)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (template == null) return ServiceResult.Fail("Протокол не найден");

        using var transaction = await _database.Database.BeginTransactionAsync();

        foreach (var verificationMethod in template.VerificationMethods!)
        {
            verificationMethod.ProtocolTemplateId = null;
        }

        _database.ProtocolTemplates.Remove(template);
        await _database.SaveChangesAsync();
        await transaction.CommitAsync();

        return ServiceResult.Success("Протокол удален");
    }

    public async Task<ServiceResult> AddVerificationMethodAsync(Guid templateId, Guid verificationMethodId)
    {
        var template = await _database.ProtocolTemplates.FindAsync(templateId);
        if (template == null) return ServiceResult.Fail("Протокол не найден");

        var verificationMethod = await _database.VerificationMethods.FindAsync(verificationMethodId);
        if (verificationMethod == null) return ServiceResult.Fail("Методика проверки не найдена");

        verificationMethod.ProtocolTemplateId = template.Id;
        await _database.SaveChangesAsync();
        _keeper.Signal(BackgroundEvents.ChangedProtocolTemplate);

        return ServiceResult.Success("Методика проверки добавлена");
    }

    public async Task<ServicePaginatedResult<PossibleTemplateVerificationMethodsDTO>> GetPossibleVerificationMethodsAsync(int pageIndex, int pageSize)
    {
        var dtoList = await _database.ProtocolTemplates
            .SelectMany(template =>
                _database.SuccessInitialVerifications
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
                            ProtocolId = template.Id,
                            ProtocolGroup = template.ProtocolGroup,
                            VerificationMethod = vm
                        })
            )
            .Where(dto => dto.VerificationMethod.ProtocolTemplateId != dto.ProtocolId)
            .ToListAsync();



        var result = dtoList.ToPaginated(pageIndex, pageSize);

        return ServicePaginatedResult<PossibleTemplateVerificationMethodsDTO>.Success(result);
    }
}

public class PossibleTemplateVerificationMethodsDTO
{
    public required Guid ProtocolId { get; init; }
    public required ProtocolGroup ProtocolGroup { get; init; }
    public required VerificationMethod VerificationMethod { get; init; }
}
