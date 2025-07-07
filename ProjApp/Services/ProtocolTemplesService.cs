using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
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
        if (result.DuplicateCount > 0) return ServiceResult.Fail("Протокол с номером типа устройства существует");
        _keeper.Signal(BackgroundEvents.NewProtocolTemplate);
        return ServiceResult.Success("Протокол добавлен");
    }

    public async Task<ServicePaginatedResult<PossibleProtocolTemplateResultDTO>> GetPossibleTemplatesAsync(int pageIndex, int pageSize)
    {
        var existsTemplates = await _database.ProtocolTemplates
            .Select(x => new
            {
                x.DeviceTypeNumbers,
                Aliases = x.VerificationMethods!.SelectMany(y => y.Aliases)
            })
            .ToListAsync();

        var existsVerificationMethods = await _database.VerificationMethods
            .Select(vm => new { vm.Id, vm.Aliases })
            .ToListAsync();

        var result = _database.InitialVerificationsSuccess
            .ProjectToType<PossibleTemplatePreDTO>(_mapper.Config)
            .AsEnumerable()
            .Where(dto => existsVerificationMethods.Any(evm => evm.Aliases.Any(evma => dto.VerificationTypeNames.Contains(evma))))
            .Where(dto => existsTemplates.All(pair => !pair.DeviceTypeNumbers.Contains(dto.DeviceTypeNumber) &&
                                                      !pair.Aliases.Any(pa => dto.VerificationTypeNames.Contains(pa))))
            .Select(dto => new PossibleProtocolTemplateResultDTO
            {
                DeviceTypeInfo = dto.DeviceTypeInfo,
                DeviceTypeNumber = dto.DeviceTypeNumber,
                VerificationTypeNames = dto.VerificationTypeNames,
                VerificationMethodIds = existsVerificationMethods.Where(vm => vm.Aliases.Any(vma => dto.VerificationTypeNames.Contains(vma)))
                                                                 .Select(vm => vm.Id)
                                                                 .Order()
                                                                 .ToArray()
            })
            .DistinctBy(dto => string.Join('|', dto.VerificationMethodIds))
            .ToPaginated(pageIndex, pageSize);

        return ServicePaginatedResult<PossibleProtocolTemplateResultDTO>.Success(result);
    }

    public async Task<ServiceResult> DeleteProtocolAsync(int id)
    {
        var template = await _database.ProtocolTemplates.FindAsync(id);
        if (template == null) return ServiceResult.Fail("Протокол не найден");
        _database.ProtocolTemplates.Remove(template);
        await _database.SaveChangesAsync();
        return ServiceResult.Success("Протокол удален");
    }
}

public class PossibleTemplatePreDTO : IRegister
{
    public required string DeviceTypeNumber { get; init; }
    public required IReadOnlyList<string> VerificationTypeNames { get; init; }
    public required string DeviceTypeInfo { get; init; }

    public void Register(TypeAdapterConfig config)
    {
        config.ForType<SuccessInitialVerification, PossibleTemplatePreDTO>()
            .Map(dest => dest.DeviceTypeInfo, src => $"{src.Device!.DeviceType!.Title} {src.Device!.DeviceType!.Notation}");
    }
}

public class PossibleProtocolTemplateResultDTO
{
    public required string DeviceTypeNumber { get; init; }
    public required IReadOnlyList<string> VerificationTypeNames { get; init; }
    public required string DeviceTypeInfo { get; init; }
    public required IReadOnlyList<Guid> VerificationMethodIds { get; init; }
}
