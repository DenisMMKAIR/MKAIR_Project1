using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class ProtocolsService
{
    private readonly ProjDatabase _database;
    private readonly AddProtocolCommand _addCommand;

    public ProtocolsService(ProjDatabase database, AddProtocolCommand addCommand)
    {
        _database = database;
        _addCommand = addCommand;
    }

    public async Task<ServicePaginatedResult<Protocol>> GetProtocolsAsync(int pageNumber, int pageSize)
    {
        var result = await _database.Protocols.ToPaginatedAsync(pageNumber, pageSize);
        return ServicePaginatedResult<Protocol>.Success(result);
    }
}
