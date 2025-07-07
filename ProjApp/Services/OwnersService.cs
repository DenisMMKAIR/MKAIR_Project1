using Microsoft.Extensions.Logging;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Database.Normalizers;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class OwnersService
{
    private readonly ProjDatabase _database;
    private readonly AddOwnerCommand _addOwnerCommand;
    private readonly ILogger<OwnersService> _logger;
    private readonly EventKeeper _keeper;

    public OwnersService(
        ProjDatabase database,
        AddOwnerCommand addOwnerCommand,
        ILogger<OwnersService> logger,
        EventKeeper keeper)
    {
        _database = database;
        _addOwnerCommand = addOwnerCommand;
        _logger = logger;
        _keeper = keeper;
    }

    public async Task<ServiceResult> AddOwner(Owner owner)
    {
        if (string.IsNullOrWhiteSpace(owner.Name))
        {
            return ServiceResult.Fail("Имя не может быть пустым");
        }

        var norm = new SpaceNormalizer();
        owner.Name = norm.Normalize(owner.Name).Trim().ToUpper();

        if (owner.Name.Length < 7)
        {
            return ServiceResult.Fail("Имя должно быть не менее 7 символов");
        }

        if (owner.INN == 0)
        {
            return ServiceResult.Fail("ИНН не может быть пустым");
        }

        var result = await _addOwnerCommand.ExecuteAsync(owner);
        if (result.Error != null) return ServiceResult.Fail(result.Error);
        if (result.DuplicateCount > 0) return ServiceResult.Fail("Владелец уже существует");

        _keeper.Signal(BackgroundEvents.ChangedOwner);

        return ServiceResult.Success("Владелец добавлен");
    }

    public async Task<ServiceResult> SetOwnerINN(Guid id, ulong inn)
    {
        if (inn == 0)
        {
            return ServiceResult.Fail("ИНН не может быть пустым");
        }

        var owner = await _database.Owners.FindAsync(id);
        if (owner == null) return ServiceResult.Fail("Владелец не найден");
        owner.INN = inn;
        await _database.SaveChangesAsync();

        _keeper.Signal(BackgroundEvents.ChangedOwner);

        return ServiceResult.Success("Владелец изменен");
    }

    public async Task<ServicePaginatedResult<Owner>> GetOwners(int page, int pageSize)
    {
        var result = await _database.Owners.ToPaginatedAsync(page, pageSize);
        return ServicePaginatedResult<Owner>.Success(result);
    }
}
