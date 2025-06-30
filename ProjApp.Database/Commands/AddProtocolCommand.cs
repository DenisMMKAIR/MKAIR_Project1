using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

[Obsolete("Reimplement")]
public class AddProtocolCommand : AddWithUniqConstraintCommand<Protocol>
{
    public AddProtocolCommand(ILogger<Protocol> logger,
         ProjDatabase db) :
         base(logger, db, new ProtocolUniqComparer())
    { }
}

public class ProtocolUniqComparer : IEqualityComparer<Protocol>
{
    public bool Equals(Protocol? x, Protocol? y)
    {
        if (x == null || y == null) return false;

        return x.DeviceTypeNumber == y.DeviceTypeNumber;
    }

    public int GetHashCode([DisallowNull] Protocol obj)
    {
        return HashCode.Combine(obj.DeviceTypeNumber);
    }
}
