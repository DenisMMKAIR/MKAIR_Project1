using System.Diagnostics.CodeAnalysis;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class VerificationUniqComparer : IEqualityComparer<IVerificationBase>
{
    public bool Equals(IVerificationBase? x, IVerificationBase? y)
    {
        if (x == null || y == null) return false;

        return x.VerificationDate.Equals(y.VerificationDate) &&
            x.DeviceTypeNumber.Equals(y.DeviceTypeNumber) &&
            x.DeviceSerial.Equals(y.DeviceSerial);
    }

    public int GetHashCode([DisallowNull] IVerificationBase obj)
    {
        return HashCode.Combine(obj.VerificationDate, obj.DeviceTypeNumber, obj.DeviceSerial);
    }

    public static VerificationUniqComparer Instance { get; } = new();
}
