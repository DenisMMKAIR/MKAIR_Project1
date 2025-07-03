using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Mapping;
using ProjApp.Normalizers;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public partial class VerificationMethodsService
{
    private readonly ProjDatabase _database;
    private readonly IMapper _mapper;
    private readonly AddVerificationMethodCommand _addCommand;
    [GeneratedRegex(@"[<>:""/\\|?*\x00-\x1F]")] private static partial Regex _invalidFileChars();

    public VerificationMethodsService(ProjDatabase database, IMapper mapper, AddVerificationMethodCommand addCommand)
    {
        _database = database;
        _mapper = mapper;
        _addCommand = addCommand;
    }

    public async Task<ServicePaginatedResult<VerificationMethodDTO>> GetVerificationMethodsAsync(int pageNumber, int pageSize)
    {
        var result = await _database.VerificationMethods
            .ProjectToType<VerificationMethodDTO>(_mapper.Config)
            .ToPaginatedAsync(pageNumber, pageSize);

        return ServicePaginatedResult<VerificationMethodDTO>.Success(result);
    }

    public Task<ServicePaginatedResult<PossibleVerificationMethodDTO>> GetPossibleVerificationMethodsAsync(int pageNumber, int pageSize, string? filterByName = null)
    {
        var existsNames = _database.VerificationMethods
            .SelectMany(v => v.Aliases)
            .Order()
            .ToImmutableSortedSet();

        var stringNormalizer = new ComplexStringNormalizer();
        filterByName = filterByName != null ? stringNormalizer.Normalize(filterByName) : string.Empty;

        var result = _database
            .InitialVerifications
            .ProjectToType<PossibleVerificationMethodDTO>(_mapper.Config)
            .Union(_database
            .FailedInitialVerifications
            .ProjectToType<PossibleVerificationMethodDTO>(_mapper.Config))
            .AsEnumerable()
            .Select(v => { v.Name = stringNormalizer.Normalize(v.Name); return v; }) // TODO: Check and clean. It must be done already
            .Where(v => v.Name.Contains(filterByName) && !existsNames.TryGetValue(v.Name, out _))
            .DistinctBy(v => (v.Name, v.DeviceTypeNumber))
            .OrderBy(v => v.Name)
            .ToPaginated(pageNumber, pageSize);

        return Task.FromResult(ServicePaginatedResult<PossibleVerificationMethodDTO>.Success(result));
    }

    public async Task<ServiceItemResult<FileDTO>> DownloadFileAsync(Guid verificationMethodId)
    {
        var vm = await _database.VerificationMethods.FirstOrDefaultAsync(v => v.Id == verificationMethodId);
        if (vm == null) return ServiceItemResult<FileDTO>.Fail("Метод поверки не найден");
        return ServiceItemResult<FileDTO>.Success(new(vm.FileName, vm.FileContent));
    }

    public async Task<ServiceResult> AddVerificationMethodAsync(VerificationMethod verificationMethod)
    {
        if (verificationMethod.Aliases == null || verificationMethod.Aliases.Count == 0)
        {
            return ServiceResult.Fail("Не указаны псевдонимы");
        }

        if (verificationMethod.Aliases.GroupBy(a => a).Any(g => g.Count() > 1))
        {
            return ServiceResult.Fail("Псевдонимы должны быть уникальными");
        }

        var stringNormalizer = new ComplexStringNormalizer();
        verificationMethod.Aliases = verificationMethod.Aliases
            .Select(stringNormalizer.Normalize)
            .Order()
            .ToArray();

        if (string.IsNullOrWhiteSpace(verificationMethod.Description) || verificationMethod.Description.Length < 3)
        {
            return ServiceResult.Fail("Описание не указано или слишком короткое описание");
        }

        verificationMethod.Description = stringNormalizer.Normalize(verificationMethod.Description);
        var sanitazedFileName = SanitizeFileName(verificationMethod.FileName);

        if (verificationMethod.FileName.Length < 3)
        {
            return ServiceResult.Fail("Некорректное имя файла");
        }

        if (verificationMethod.FileContent == null || verificationMethod.FileContent.Length < 3)
        {
            return ServiceResult.Fail("Файл пустой или некорректный");
        }

        if (verificationMethod.FileContent.Length > 10 * 1024 * 1024)
        {
            return ServiceResult.Fail("Не удалось добавить файл. Лимит 10МБ");
        }

        var result = await _addCommand.ExecuteAsync(verificationMethod);
        if (result.Error != null) return ServiceResult.Fail(result.Error);
        if (result.NewCount!.Value == 0) return ServiceResult.Fail("Метод поверки с псевдонимом уже существует");
        return ServiceResult.Success("Метод поверки добавлен");
    }

    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return string.Empty;
        return _invalidFileChars().Replace(fileName.Trim(), "_");
    }
}
