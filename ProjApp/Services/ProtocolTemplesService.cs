using Mapster;
using MapsterMapper;
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
}
