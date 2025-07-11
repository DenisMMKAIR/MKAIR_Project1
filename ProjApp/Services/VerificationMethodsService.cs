using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Database.Normalizers;
using ProjApp.Database.SupportTypes;
using ProjApp.Mapping;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public partial class VerificationMethodsService
{
    private readonly ILogger<VerificationMethodsService> _logger;
    private readonly ProjDatabase _database;
    private readonly IMapper _mapper;
    private readonly AddVerificationMethodCommand _addCommand;
    [GeneratedRegex(@"[<>:""/\\|?*\x00-\x1F]")] private static partial Regex _invalidFileChars();

    public VerificationMethodsService(ILogger<VerificationMethodsService> logger, ProjDatabase database, IMapper mapper, AddVerificationMethodCommand addCommand)
    {
        _logger = logger;
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

    public Task<ServicePaginatedResult<PossibleVerificationMethodDTO>> GetPossibleVerificationMethodsAsync(int pageNumber, int pageSize, string? deviceTypeNumberFilter = null, string? verificationNameFilter = null, string? deviceTypeInfoFilter = null, YearMonth? yearMonthFilter = null)
    {
        var existsNames = _database.VerificationMethods
            .SelectMany(v => v.Aliases)
            .ToImmutableSortedSet();

        var stringNormalizer = new ComplexStringNormalizer();
        deviceTypeNumberFilter = deviceTypeNumberFilter != null ? stringNormalizer.Normalize(deviceTypeNumberFilter) : string.Empty;
        verificationNameFilter = verificationNameFilter != null ? stringNormalizer.Normalize(verificationNameFilter) : string.Empty;
        deviceTypeInfoFilter = deviceTypeInfoFilter != null ? stringNormalizer.Normalize(deviceTypeInfoFilter) : string.Empty;

        var query = _database
            .SuccessInitialVerifications
            .ProjectToType<PossibleVerificationMethodPreSelectDTO>(_mapper.Config)
            .Union(_database
            .FailedInitialVerifications
            .ProjectToType<PossibleVerificationMethodPreSelectDTO>(_mapper.Config))
            .Union(_database
            .SuccessVerifications
            .ProjectToType<PossibleVerificationMethodPreSelectDTO>(_mapper.Config))
            .Union(_database
            .FailedVerifications
            .ProjectToType<PossibleVerificationMethodPreSelectDTO>(_mapper.Config))
            .AsEnumerable()
            .GroupBy(dto => dto.DeviceTypeNumber)
            .Select(g => g.Adapt<PossibleVerificationMethodDTO>(_mapper.Config));

        if (yearMonthFilter != null)
        {
            query = query.Where(dto => dto.Dates.Any(d => d == yearMonthFilter));
        }

        var result = query
            .Where(dto => stringNormalizer.Normalize(dto.DeviceTypeNumber).Contains(deviceTypeNumberFilter))
            .Where(dto => stringNormalizer.Normalize(dto.DeviceTypeInfo).Contains(deviceTypeInfoFilter))
            .Where(dto => dto.VerificationTypeNames.Any(name => name.Contains(verificationNameFilter)))
            .Where(dto => dto.VerificationTypeNames.Any(vn => !existsNames.TryGetValue(vn, out _)))
            .OrderByDescending(dto => dto.VerificationTypeNames.Count)
            .ThenBy(dto => dto.DeviceTypeNumber)
            .ThenBy(dto => dto.Dates[0])
            .ToPaginated(pageNumber, pageSize);

        return Task.FromResult(ServicePaginatedResult<PossibleVerificationMethodDTO>.Success(result));
    }

    public async Task<ServiceItemResult<VerificationMethodFile>> DownloadFileAsync(Guid fileId)
    {
        var file = await _database.VerificationMethodFiles.FirstOrDefaultAsync(f => f.Id == fileId);
        if (file == null) return ServiceItemResult<VerificationMethodFile>.Fail("Файл не найден");
        return ServiceItemResult<VerificationMethodFile>.Success(file);
    }

    public async Task<ServiceResult> DeleteVerificationMethodAsync(Guid VerificationMethodId)
    {
        var m = await _database.VerificationMethods
            .Include(m => m.ProtocolTemplate)
            .Include(m => m.DeviceTypes)
            .FirstOrDefaultAsync(m => m.Id == VerificationMethodId);

        if (m == null) return ServiceResult.Fail("Метод поверки не найден");

        await using var transaction = await _database.Database.BeginTransactionAsync();

        try
        {
            foreach (var dt in m.DeviceTypes!) dt.VerificationMethodId = null;
            _database.VerificationMethods.Remove(m);
            await _database.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при удалении");
            await transaction.RollbackAsync();
            return ServiceResult.Fail("Ошибка при удалении");
        }

        return ServiceResult.Success("Метод поверки удален");
    }

    public async Task<ServiceResult> AddAliasesAsync(IReadOnlyList<string> aliases, Guid verificationMethodId)
    {
        var m = await _database.VerificationMethods.FindAsync(verificationMethodId);

        if (m == null) return ServiceResult.Fail("Метод поверки не найден");

        var stringNormalizer = new ComplexStringNormalizer();

        var newAliases = aliases
            .Select(stringNormalizer.Normalize)
            .Except(m.Aliases)
            .ToArray();

        if (newAliases.Length == 0) return ServiceResult.Success("Поверка уже имеет указанные псевдонимы");

        m.Aliases = m.Aliases.Concat(newAliases)
            .OrderBy(a => a.Length)
            .ToArray();

        using var transaction = await _database.Database.BeginTransactionAsync();
        var (deviceTypeCount, newAliasesCount) = await AddAllDevicesAsync(m, stringNormalizer, transaction);
        await _database.SaveChangesAsync();
        await transaction.CommitAsync();

        return ServiceResult.Success($"Псевдонимы добавлены: {newAliases.Length}. Устройства присвоены: {deviceTypeCount}.");
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

        if (verificationMethod.Checkups.Count == 0)
        {
            return ServiceResult.Fail("Не указаны пункты проверки");
        }

        var stringNormalizer = new ComplexStringNormalizer();
        verificationMethod.Aliases = verificationMethod.Aliases
            .Select(stringNormalizer.Normalize)
            .OrderBy(a => a.Length)
            .ToArray();

        if (string.IsNullOrWhiteSpace(verificationMethod.Description) || verificationMethod.Description.Length < 3)
        {
            return ServiceResult.Fail("Описание не указано или слишком короткое описание");
        }

        verificationMethod.Description = stringNormalizer.Normalize(verificationMethod.Description);

        if (verificationMethod.VerificationMethodFiles != null)
        {
            if (verificationMethod.VerificationMethodFiles.Any(fc => fc.Content.Length > 10 * 1024 * 1024))
            {
                return ServiceResult.Fail("Не удалось добавить файл. Лимит 10МБ");
            }

            foreach (var file in verificationMethod.VerificationMethodFiles!)
            {
                file.FileName = SanitizeFileName(file.FileName);

                if (file.FileName.Length < 5)
                {
                    return ServiceResult.Fail("Некорректное имя файла");
                }
            }
        }

        using var transaction = await _database.Database.BeginTransactionAsync();

        var result = await _addCommand.ExecuteAsync([verificationMethod], transaction);

        if (result.Error != null) return ServiceResult.Fail(result.Error);
        if (result.NewCount!.Value == 0) return ServiceResult.Fail("Метод поверки с псевдонимом уже существует");

        var (deviceTypesCount, newAliasesCount) = await AddAllDevicesAsync(verificationMethod, stringNormalizer, transaction);

        await transaction.CommitAsync();

        return ServiceResult.Success($"Метод поверки добавлен. Присвоен устройствам {deviceTypesCount}");
    }

    private async Task<(int deviceTypesCount, int newAliasesCount)> AddAllDevicesAsync(VerificationMethod verificationMethod, ComplexStringNormalizer stringNormalizer, IDbContextTransaction? transaction)
    {
        int deviceTypesCount = 0, newAliasesCount = 0;

        while (true)
        {
            var dtos = _database
                .SuccessInitialVerifications
                .ProjectToType<PossibleVerificationMethodPreSelectDTO>(_mapper.Config)
                .Union(_database
                .FailedInitialVerifications
                .ProjectToType<PossibleVerificationMethodPreSelectDTO>(_mapper.Config))
                .Union(_database
                .SuccessVerifications
                .ProjectToType<PossibleVerificationMethodPreSelectDTO>(_mapper.Config))
                .Union(_database
                .FailedVerifications
                .ProjectToType<PossibleVerificationMethodPreSelectDTO>(_mapper.Config))
                .AsEnumerable()
                .GroupBy(dto => dto.DeviceTypeNumber)
                .Select(g => g.Adapt<PossibleVerificationMethodDTO>(_mapper.Config))
                .Where(dto => dto.VerificationMethodId == null)
                .Where(dto => dto.VerificationTypeNames.Any(dtoA => verificationMethod.Aliases.Contains(dtoA)))
                .Select(dto => new { dto.DeviceTypeNumber, dto.VerificationTypeNames })
                .ToArray();

            var deviceTypes = _database.DeviceTypes
                .AsEnumerable()
                .Where(dt => dtos.Any(dto => dto.DeviceTypeNumber == dt.Number))
                .ToArray();

            foreach (var deviceType in deviceTypes) deviceType.VerificationMethod = verificationMethod;

            var newAliases = dtos
                .SelectMany(dto => dto.VerificationTypeNames)
                .Select(stringNormalizer.Normalize)
                .Where(a => !verificationMethod.Aliases.Contains(a))
                .ToArray();

            verificationMethod.Aliases = verificationMethod.Aliases
                .Concat(newAliases)
                .Distinct()
                .Order()
                .ToArray();

            if (deviceTypes.Length == 0 && newAliases.Length == 0) break;

            deviceTypesCount += deviceTypes.Length;
            newAliasesCount += newAliases.Length;

            await _database.SaveChangesAsync();
        }

        return (deviceTypesCount, newAliasesCount);
    }

    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return string.Empty;
        return _invalidFileChars().Replace(fileName.Trim(), "_");
    }
}
