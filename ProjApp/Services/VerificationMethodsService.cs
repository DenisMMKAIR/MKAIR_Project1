using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Mapping;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public partial class VerificationMethodsService
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
        var existsNames = await _database.Set<VerificationMethodAlias>().Select(v => v.Name).ToListAsync();

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
        if (verificationMethod.Aliases == null || verificationMethod.Aliases.Count == 0)
        {
            return ServiceResult.Fail("Не указаны псевдонимы");
        }

        if (verificationMethod.Aliases.GroupBy(a => a.Name).Any(g => g.Count() > 1))
        {
            return ServiceResult.Fail("Псевдонимы должны быть уникальными");
        }

        if (string.IsNullOrWhiteSpace(verificationMethod.Description) || verificationMethod.Description.Length < 3)
        {
            return ServiceResult.Fail("Описание не указано или слишком короткое описание");
        }

        var sanitazedFileName = SanitizeFileName(verificationMethod.FileName);

        if (verificationMethod.FileName.Length < 3)
        {
            return ServiceResult.Fail("Некорректное имя файла");
        }

        if (verificationMethod.FileContent == null || verificationMethod.FileContent.Length < 3)
        {
            return ServiceResult.Fail("Файл пустой или некорректный");
        }

        var result = await _addCommand.ExecuteAsync(verificationMethod);
        if (result.Error != null) return ServiceResult.Fail(result.Error);
        if (result.NewCount!.Value == 0) return ServiceResult.Fail("Такой метод поверки уже существует");
        return ServiceResult.Success("Метод поверки добавлен");
    }

    [GeneratedRegex(@"[<>:""/\\|?*\x00-\x1F]")]
    private static partial Regex _invalidFileChars();
    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return string.Empty;
        return _invalidFileChars().Replace(fileName.Trim(), "_");
    }
}
