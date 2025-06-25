using System.Diagnostics.CodeAnalysis;

namespace ProjApp.Database.Entities;

public class DeviceType : DatabaseEntity
{
    public required string Number { get; set; }
    public required string Name { get; set; }
    public required string Notation { get; set; }
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
