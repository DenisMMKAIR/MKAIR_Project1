using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddDeviceTypeCommand : AddWithUniqConstraintCommand<DeviceType>
{
    public AddDeviceTypeCommand(ILogger<AddWithUniqConstraintCommand<DeviceType>> logger,
         ProjDatabase db) :
         base(logger, db, new DeviceTypeUniqComparer())
    { }
}

public class DeviceTypeUniqComparer : IEqualityComparer<DeviceType>
{
    public bool Equals(DeviceType? x, DeviceType? y)
    {
        if (x == null || y == null) return false;

        return x.Number.Equals(y.Number);
    }

    public int GetHashCode([DisallowNull] DeviceType obj)
    {
        return HashCode.Combine(obj.Number);
    }
}
