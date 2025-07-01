using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddProtocolCheckupCommand : AddWithUniqConstraintCommand<ProtocolCheckup>
{
    public AddProtocolCheckupCommand(ILogger<ProtocolCheckup> logger,
         ProjDatabase db) :
         base(logger, db, new ProtocolCheckupUniqComparer())
    { }
}

public class ProtocolCheckupUniqComparer : IEqualityComparer<ProtocolCheckup>
{
    public bool Equals(ProtocolCheckup? x, ProtocolCheckup? y)
    {
        if (x == null || y == null) return false;

        return x.Name == y.Name;
    }

    public int GetHashCode([DisallowNull] ProtocolCheckup obj)
    {
        return HashCode.Combine(obj.Name);
    }
}
