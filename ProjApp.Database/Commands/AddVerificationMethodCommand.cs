using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddVerificationMethodCommand : AddWithUniqConstraintCommand<VerificationMethod>
{
    public AddVerificationMethodCommand(ILogger<VerificationMethod> logger,
         ProjDatabase db) :
         base(logger, db, VerificationMethodUniqComparer.Instance)
    {
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
        foreach (var alias in obj.Aliases.Order())
        {
            hash.Add(alias);
        }
        return hash.ToHashCode();
    }

    public static VerificationMethodUniqComparer Instance { get; } = new VerificationMethodUniqComparer();
}

public static class CollectionExtensions
{
    public static void ThrowIfNullOrEmpty<T>(this IReadOnlyList<T>? collection, string message)
    {
        ArgumentNullException.ThrowIfNull(collection, message);
        if (collection.Count == 0) throw new ArgumentException(message);
    }
}
