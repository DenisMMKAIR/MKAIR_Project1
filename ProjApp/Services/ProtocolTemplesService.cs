using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
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

    public ProtocolTemplesService(ProjDatabase database, IMapper mapper, AddProtocolTemplateCommand addCommand)
    {
        _database = database;
        _mapper = mapper;
        _addCommand = addCommand;
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
        return ServiceResult.Success("Протокол добавлен");
    }

    public async Task<ServicePaginatedResult<PossibleTemplateDTO>> GetPossibleTemplatesAsync(int pageIndex, int pageSize)
    {
        var existsAliases = await _database.VerificationMethods.SelectMany(x => x.Aliases).ToListAsync();
        var existsTemplates = await _database.ProtocolTemplates
            .Select(x => new
            {
                x.DeviceTypeNumber,
                Aliases = x.VerificationMethods!.SelectMany(y => y.Aliases)
            })
            .ToListAsync();

        var result = _database.InitialVerifications
            .Where(iv => existsAliases.Contains(iv.VerificationTypeName))
            .ProjectToType<PossibleTemplateDTO>(_mapper.Config)
            .AsEnumerable()
            // .Where(dto => existsTemplates.All(pair => pair.DeviceTypeNumber != dto.DeviceTypeNumber && !pair.Aliases.Contains(dto.VerificationTypeName)))
            .DistinctBy(dto => (dto.DeviceTypeNumber, dto.VerificationTypeName))
            .OrderBy(dto => dto.DeviceTypeNumber)
            .ToPaginated(pageIndex, pageSize);

        return ServicePaginatedResult<PossibleTemplateDTO>.Success(result);
    }
}

public class PossibleTemplateDTO : IRegister
{
    public required string DeviceTypeNumber { get; init; }
    public required string VerificationTypeName { get; init; }
    public required string DeviceTypeInfo { get; init; }

    public void Register(TypeAdapterConfig config)
    {
        config.ForType<InitialVerification, PossibleTemplateDTO>()
            .Map(dest => dest.DeviceTypeInfo, src => $"{src.Device!.DeviceType!.Title} {src.Device!.DeviceType!.Notation}");
    }
}
