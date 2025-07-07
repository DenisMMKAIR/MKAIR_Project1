using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddOwnerCommand : AddWithUniqConstraintCommand<Owner>
{
    public AddOwnerCommand(ILogger<AddOwnerCommand> logger,
         ProjDatabase db) :
         base(logger, db, new OwnerUniqComparer())
    { }
}

public class OwnerUniqComparer : IEqualityComparer<Owner>
{
    public bool Equals(Owner? x, Owner? y)
    {
        if (x == null || y == null) return false;

        return x.Name == y.Name;
    }

    public int GetHashCode([DisallowNull] Owner obj)
    {
        return obj.Name.GetHashCode();
    }
}
