using Microsoft.EntityFrameworkCore;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Mapping;

namespace ProjApp.Services;

public class VerificationMethodsService
{
    private readonly ProjDatabase _database;
    private readonly AddVerificationMethodCommand _addCommand;

    public VerificationMethodsService(ProjDatabase database, AddVerificationMethodCommand addCommand)
    {
        _database = database;
        _addCommand = addCommand;
    }

    public async Task<ServicePaginatedResult<VerificationMethod>> GetVerificationMethodsAsync(int pageNumber, int pageSize)
    {
        var result = await _database.VerificationMethods.ToPaginatedAsync(pageNumber, pageSize);
        return ServicePaginatedResult<VerificationMethod>.Success(result);
    }

    public async Task<ServicePaginatedResult<PossibleVerificationMethodDTO>> GetPossibleVerificationMethodsAsync(int pageNumber, int pageSize, string? filterByName = null)
    {
        var existsNames = await _database.VerificationMethods.Select(v => v.Name).Distinct().ToListAsync();

        var dtos1 = await _database
            .InitialVerifications
            .Where(v => v.VerificationTypeName.Contains(filterByName ?? "", StringComparison.CurrentCultureIgnoreCase))
            .Include(v => v.Device)
            .ThenInclude(d => d!.DeviceType)
            .Map(v => PossibleVerificationMethodDTO.MapTo(v))
            .ToArrayAsync();

        var dtos2 = await _database.FailedInitialVerifications
            .Where(v => v.VerificationTypeName.Contains(filterByName ?? "", StringComparison.CurrentCultureIgnoreCase))
            .Include(v => v.Device)
            .ThenInclude(d => d!.DeviceType)
            .Map(v => PossibleVerificationMethodDTO.MapTo(v))
            .ToArrayAsync();

        var result = dtos1.Concat(dtos2)
            .Where(v => !existsNames.Contains(v.Name))
            .DistinctBy(v => (v.Name, v.DeviceTypeNumber))
            .OrderBy(v => v.Name)
            .ToPaginated(pageNumber, pageSize);

        return ServicePaginatedResult<PossibleVerificationMethodDTO>.Success(result);
    }

    public async Task<ServiceResult> AddVerificationMethodAsync(VerificationMethod verificationMethod)
    {
        if (string.IsNullOrWhiteSpace(verificationMethod.Name) || verificationMethod.Name.Length < 3)
        {
            return ServiceResult.Fail("Имя не указано или слишком короткое имя");
        }

        if (string.IsNullOrWhiteSpace(verificationMethod.Description) || verificationMethod.Description.Length < 3)
        {
            return ServiceResult.Fail("Описание не указано или слишком короткое описание");
        }

        if (verificationMethod.Aliases == null || verificationMethod.Aliases.Count == 0)
        {
            return ServiceResult.Fail("Не указаны псевдонимы");
        }

        if (verificationMethod.Checkups == null || verificationMethod.Checkups.Count == 0)
        {
            return ServiceResult.Fail("Не указаны пункты проверки");
        }

        var result = await _addCommand.ExecuteAsync(verificationMethod);
        if (result.Error != null) return ServiceResult.Fail(result.Error);
        var msg = result.Message == "Добавлено 1 новых элементов" ?
            "Метод поверки добавлен" :
            "Такой метод поверки уже существует";
        return ServiceResult.Success(msg);
    }
}
