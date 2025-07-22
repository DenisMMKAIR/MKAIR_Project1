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
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;
using ProjApp.Mapping;
using ProjApp.Normalizers;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public partial class VerificationMethodsService
{
    private readonly ILogger<VerificationMethodsService> _logger;
    private readonly ProjDatabase _database;
    private readonly IMapper _mapper;
    private readonly AddVerificationMethodCommand _addCommand;

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

    public async Task<ServicePaginatedResult<PossibleVrfMethodDTO>> GetPossibleVerificationMethodsAsync(
        int pageNumber,
        int pageSize,
        ShowVMethods showVMethods,
        string? deviceTypeNumberFilter = null,
        string? verificationNameFilter = null,
        string? deviceTypeInfoFilter = null,
        YearMonth? yearMonthFilter = null,
        CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;
        var aliasNormComp = VerificationMethodAliasComparerNormalizer.Instance.Normalize;

        var dbNormAliases = _database.VerificationMethods
            .SelectMany(v => v.Aliases)
            .Select(aliasNormComp)
            .ToImmutableSortedSet();

        if (cancellationToken.Value.IsCancellationRequested)
        {
            return ServicePaginatedResult<PossibleVrfMethodDTO>.Fail("Операция отменена");
        }

        var serverResult = await _database.SuccessInitialVerifications
            .Select(v => new
            {
                v.DeviceTypeNumber,
                v.Device!.DeviceType!.Title,
                v.Device.DeviceType.Notation,
                v.Device.Modification,
                v.VerificationTypeName,
                v.VerificationDate,
                v.Device.DeviceType.MethodUrls,
                v.Device.DeviceType.SpecUrls,
            })
            .Union(_database.FailedInitialVerifications
                .Select(v => new
                {
                    v.DeviceTypeNumber,
                    v.Device!.DeviceType!.Title,
                    v.Device.DeviceType.Notation,
                    v.Device.Modification,
                    v.VerificationTypeName,
                    v.VerificationDate,
                    v.Device.DeviceType.MethodUrls,
                    v.Device.DeviceType.SpecUrls,
                }))
            .Union(_database.Manometr1Verifications
                .Select(v => new
                {
                    v.DeviceTypeNumber,
                    v.Device!.DeviceType!.Title,
                    v.Device.DeviceType.Notation,
                    v.Device.Modification,
                    VerificationTypeName = v.InitialVerificationName,
                    v.VerificationDate,
                    v.Device.DeviceType.MethodUrls,
                    v.Device.DeviceType.SpecUrls,
                }))
            .ToArrayAsync(cancellationToken.Value);

        var clientQuery = serverResult
            .GroupBy(v => v.DeviceTypeNumber)
            .Select(g => new PossibleVrfMethodDTO
            (
                DeviceTypeNumber: g.Key,
                DeviceTypeInfo: g.Select(dto => $"{dto.Title} {dto.Notation}").First(),
                MethodUrls: g.SelectMany(dto => dto.MethodUrls ?? [])
                    .Distinct()
                    .ToArray(),
                SpecUrls: g.SelectMany(dto => dto.SpecUrls ?? [])
                    .Distinct()
                    .ToArray(),
                AliasGroups: g.Select(dto => new
                {
                    Name = dto.VerificationTypeName,
                    dto.Modification,
                    Date = dto.VerificationDate
                })
                .GroupBy(dto => aliasNormComp(dto.Name))
                .Select(g => new PossibleVrfMethodAliasGroupDTO
                (
                    Aliases: g.Select(dto => new PossibleVrfMethodAliasDTO
                    (
                        Exists: dbNormAliases.Contains(aliasNormComp(dto.Name)),
                        Alias: dto.Name
                    ))
                    .DistinctBy(dto => dto.Alias)
                    .ToArray(),
                    Modifications: g.Select(dto => dto.Modification)
                        .Distinct()
                        .OrderBy(m => m.Length)
                        .ThenBy(m => m)
                        .ToArray(),
                    Dates: g.Select(dto => (YearMonth)dto.Date)
                        .Distinct()
                        .Order()
                        .ToArray()
                ))
                .ToArray()
            ));

        switch (showVMethods)
        {
            case ShowVMethods.Новые:
                clientQuery = clientQuery.Where(dto => dto.AliasGroups.All(ag => ag.Aliases.All(a => !a.Exists)));
                break;
            case ShowVMethods.Частичные:
                clientQuery = clientQuery.Where(dto => dto.AliasGroups.Any(ag => ag.Aliases.Any(a => !a.Exists)));
                break;
        }

        if (deviceTypeNumberFilter != null)
        {
            clientQuery = clientQuery.Where(dto => dto.DeviceTypeNumber.Contains(deviceTypeNumberFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (verificationNameFilter != null)
        {
            clientQuery = clientQuery.Where(dto => dto.AliasGroups
                .Any(ag => ag.Aliases.Any(a => a.Alias.Contains(verificationNameFilter, StringComparison.OrdinalIgnoreCase))));
        }

        if (deviceTypeInfoFilter != null)
        {
            clientQuery = clientQuery.Where(dto => dto.DeviceTypeInfo.Contains(deviceTypeInfoFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (yearMonthFilter != null)
        {
            clientQuery = clientQuery.Where(dto => dto.AliasGroups.Any(ag => ag.Dates.Any(d => d == yearMonthFilter.Value)));
        }

        var result = clientQuery
            .OrderByDescending(dto => dto.AliasGroups.Count)
            .ThenBy(dto => dto.DeviceTypeNumber)
            .ThenBy(dto => dto.AliasGroups[0].Dates[0])
            .ToPaginated(pageNumber, pageSize);

        if (cancellationToken.Value.IsCancellationRequested)
        {
            return ServicePaginatedResult<PossibleVrfMethodDTO>.Fail("Операция отменена");
        }

        return ServicePaginatedResult<PossibleVrfMethodDTO>.Success(result);
    }

    public async Task<ServiceItemResult<VerificationMethodFile>> DownloadFileAsync(Guid fileId)
    {
        var file = await _database.VerificationMethodFiles.FirstOrDefaultAsync(f => f.Id == fileId);
        if (file == null) return ServiceItemResult<VerificationMethodFile>.Fail("Файл не найден");
        return ServiceItemResult<VerificationMethodFile>.Success(file);
    }

    public async Task<ServiceResult> DeleteVerificationMethodAsync(Guid VerificationMethodId)
    {
        var m = await _database.VerificationMethods.FirstOrDefaultAsync(m => m.Id == VerificationMethodId);

        if (m == null) return ServiceResult.Fail("Метод поверки не найден");

        await using var transaction = await _database.Database.BeginTransactionAsync();

        try
        {
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

        var newAliases = aliases
            .DistinctBy(VerificationMethodAliasComparerNormalizer.Instance.Normalize)
            .Select(VerificationMethodAliasVisualNormalizer.Instance.Normalize)
            .Except(m.Aliases)
            .ToArray();

        if (newAliases.Length == 0) return ServiceResult.Success("Поверка уже имеет указанные псевдонимы");

        m.Aliases = m.Aliases.Concat(newAliases)
            .OrderBy(a => a.Length)
            .ThenBy(a => a)
            .ToArray();

        using var transaction = await _database.Database.BeginTransactionAsync();

        var vrfsCount = await AddVrfAsync(m, transaction);

        await _database.SaveChangesAsync();
        await transaction.CommitAsync();

        return ServiceResult.Success($"Псевдонимы добавлены: {newAliases.Length}. Поверки присвоены: {vrfsCount}.");
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

        verificationMethod.Aliases = verificationMethod.Aliases
            .DistinctBy(VerificationMethodAliasComparerNormalizer.Instance.Normalize)
            .Select(VerificationMethodAliasVisualNormalizer.Instance.Normalize)
            .OrderBy(a => a.Length)
            .ThenBy(a => a)
            .ToArray();

        if (string.IsNullOrWhiteSpace(verificationMethod.Description) || verificationMethod.Description.Length < 3)
        {
            return ServiceResult.Fail("Описание не указано или слишком короткое описание");
        }

        verificationMethod.Description = VerificationMethodDescriptionNormalizer.Instance.Normalize(verificationMethod.Description);

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

        var vrfsCount = await AddVrfAsync(verificationMethod, transaction);

        await transaction.CommitAsync();

        return ServiceResult.Success($"Метод поверки добавлен. Присвоен поверкам {vrfsCount}");
    }

    private async Task<int> AddVrfAsync(VerificationMethod verificationMethod, IDbContextTransaction transaction)
    {
        var vrfAdded = 0;
        var norm = VerificationMethodAliasComparerNormalizer.Instance.Normalize;

        var normAliases = verificationMethod.Aliases
            .Select(norm)
            .ToImmutableSortedSet();

        foreach (var v in _database.SuccessInitialVerifications
                                   .Where(v => v.VerificationMethod == null)
                                   .AsEnumerable()
                                   .Where(v => normAliases.Contains(norm(v.VerificationTypeName))))
        {
            v.VerificationMethod = verificationMethod;
            vrfAdded++;
        }

        foreach (var v in _database.FailedInitialVerifications
                                   .Where(v => v.VerificationMethod == null)
                                   .AsEnumerable()
                                   .Where(v => normAliases.Contains(norm(v.VerificationTypeName))))
        {
            v.VerificationMethod = verificationMethod;
            vrfAdded++;
        }

        foreach (var v in _database.Manometr1Verifications
                                   .Where(v => v.VerificationMethod == null)
                                   .AsEnumerable()
                                   .Where(v => normAliases.Contains(norm(v.InitialVerificationName))))
        {
            v.VerificationMethod = verificationMethod;
            vrfAdded++;
        }

        await _database.SaveChangesAsync();

        return vrfAdded;
    }

    [GeneratedRegex(@"[<>:""/\\|?*\x00-\x1F]")] private static partial Regex _invalidFileChars();
    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return string.Empty;
        return _invalidFileChars().Replace(fileName.Trim(), "_");
    }
}
