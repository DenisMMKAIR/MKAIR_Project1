using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddVerificationMethodCommand : AddWithUniqConstraintCommand<VerificationMethod>
{
    private readonly AddVerificationMethodAliasCommand AddAliasCommand;

    public AddVerificationMethodCommand(ILogger<VerificationMethod> logger,
         ProjDatabase db,
         AddVerificationMethodAliasCommand addAliasCommand) :
         base(logger, db, new VerificationMethodUniqComparer())
    {
        AddAliasCommand = addAliasCommand;
    }

    public override async Task<Result> ExecuteAsync(params IReadOnlyList<VerificationMethod> items)
    {
        foreach (var item in items)
        {
            if (item.Aliases == null || item.Aliases.Count == 0) return Result.Failed($"У метода поверки нет псевдонимов");

            var addAliasResult = await AddAliasCommand.ExecuteAsync(item.Aliases);
            if (addAliasResult.Error != null) return Result.Failed(addAliasResult.Error);
            if (addAliasResult.DuplicateCount! > 0) return Result.Failed("Метод поверки с псевдонимом уже существует");
            item.Aliases = addAliasResult.Items!;
        }
        return await base.ExecuteAsync(items);
    }
}

public class VerificationMethodUniqComparer : IEqualityComparer<VerificationMethod>
{
    public bool Equals(VerificationMethod? x, VerificationMethod? y)
    {
        if (x == null || y == null) return false;

        x.Aliases.ThrowIfNullOrEmpty("VerificationMethod aliases can't be null or empty");
        y.Aliases.ThrowIfNullOrEmpty("VerificationMethod aliases can't be null or empty");

        return x.Aliases.Any(xa => y.Aliases.Contains(xa));
    }

    public int GetHashCode([DisallowNull] VerificationMethod obj)
    {
        obj.Aliases.ThrowIfNullOrEmpty("VerificationMethod aliases can't be null or empty");

        var hash = new HashCode();
        foreach (var alias in obj.Aliases.OrderBy(a => a.Name))
        {
            hash.Add(alias.Name);
        }
        return hash.ToHashCode();
    }
}

public static class CollectionExtensions
{
    public static void ThrowIfNullOrEmpty<T>(this IReadOnlyList<T>? collection, string message)
    {
        ArgumentNullException.ThrowIfNull(collection);
        if (collection.Count == 0) throw new ArgumentException(message);
    }
}
