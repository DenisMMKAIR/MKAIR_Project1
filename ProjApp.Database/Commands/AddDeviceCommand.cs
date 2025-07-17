using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddDeviceCommand : AddWithUniqConstraintCommand<Device>
{
    public AddDeviceCommand(ILogger<AddDeviceCommand> logger,
         ProjDatabase db) :
         base(logger, db, new DeviceUniqComparer())
    { }
}

public class DeviceUniqComparer : IEqualityComparer<Device>
{
    public bool Equals(Device? x, Device? y)
    {
        if (x == null || y == null) return false;

        return x.DeviceTypeNumber == y.DeviceTypeNumber &&
               x.Serial == y.Serial;
    }

    public int GetHashCode([DisallowNull] Device obj)
    {
        return HashCode.Combine(obj.DeviceTypeNumber, obj.Serial);
    }
    
    public static IEqualityComparer<Device> Instance { get; } = new DeviceUniqComparer();
}
