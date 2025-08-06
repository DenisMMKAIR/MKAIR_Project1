using System.Diagnostics.CodeAnalysis;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class VerificationUniqComparer : IEqualityComparer<IVerificationBase>
{
    public bool Equals(IVerificationBase? x, IVerificationBase? y)
    {
        if (x == null || y == null) return false;

        var xSerialString = x.DeviceSerial;
        var ySerialString = y.DeviceSerial;

        if (ulong.TryParse(xSerialString, out var xSerial))
        {
            xSerialString = xSerial.ToString();
        }

        if (ulong.TryParse(ySerialString, out var ySerial))
        {
            ySerialString = ySerial.ToString();
        }

        return x.VerificationDate.Equals(y.VerificationDate) &&
                x.DeviceTypeNumber.Equals(y.DeviceTypeNumber) &&
                xSerialString.Equals(ySerialString);
    }

    public int GetHashCode([DisallowNull] IVerificationBase obj)
    {
        var serialString = obj.DeviceSerial;

        if (ulong.TryParse(serialString, out var serial))
        {
            serialString = serial.ToString();
        }

        return HashCode.Combine(obj.VerificationDate, obj.DeviceTypeNumber, serialString);
    }

    public static VerificationUniqComparer Instance { get; } = new();
}


