using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

[Obsolete("Wrong logic. throw if any alias already exists")]
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
            if (item.Aliases == null || item.Aliases.Count == 0) return Result.Failed("Пустой список псевдонимов");

            var addAliasResult = await AddAliasCommand.ExecuteAsync(item.Aliases);
            if (addAliasResult.Error != null) return Result.Failed(addAliasResult.Error);
            item.Aliases = addAliasResult.SavedItems!;
        }
        return await base.ExecuteAsync(items);
    }
}

public class VerificationMethodUniqComparer : IEqualityComparer<VerificationMethod>
{
    public bool Equals(VerificationMethod? x, VerificationMethod? y)
    {
        if (x == null || y == null) return false;

        if (x.Aliases == null || y.Aliases == null || x.Aliases.Count == 0 || y.Aliases.Count == 0)
        {
            throw new ArgumentNullException("VerificationMethod aliases can't be null or empty");
        }

        return x.Aliases.Any(xa => y.Aliases.Contains(xa));
    }

    public int GetHashCode([DisallowNull] VerificationMethod obj)
    {
        if (obj.Aliases == null || obj.Aliases.Count == 0)
        {
            throw new ArgumentNullException("VerificationMethod aliases can't be null or empty");
        }

        var hash = new HashCode();
        foreach (var alias in obj.Aliases.OrderBy(a => a.Name))
        {
            hash.Add(alias.Name);
        }
        return hash.ToHashCode();
    }
}
