using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddVerificationMethodAliasCommand : AddWithUniqConstraintCommand<VerificationMethodAlias>
{
    public AddVerificationMethodAliasCommand(ILogger<VerificationMethodAlias> logger,
         ProjDatabase db) :
         base(logger, db, new VerificationMethodAliasUniqComparer()) { }
}

public class VerificationMethodAliasUniqComparer : IEqualityComparer<VerificationMethodAlias>
{
    public bool Equals(VerificationMethodAlias? x, VerificationMethodAlias? y)
    {
        if (x == null || y == null) return false;

        return x.Name == y.Name;
    }

    public int GetHashCode([DisallowNull] VerificationMethodAlias obj)
    {
        return HashCode.Combine(obj.Name);
    }
}