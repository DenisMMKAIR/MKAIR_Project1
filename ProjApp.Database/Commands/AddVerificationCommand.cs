using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddVerificationCommand<T> : AddWithUniqConstraintCommand<T> where T : DatabaseEntity, IVerification
{
    public AddVerificationCommand(
        ILogger<AddVerificationCommand<T>> logger,
        ProjDatabase db)
        : base(logger, db, new VerificationUniqComparer<T>())
    {
    }
}

public class VerificationUniqComparer<T> : IEqualityComparer<T> where T : IVerification
{
    public bool Equals(T? x, T? y)
    {
        if (x == null || y == null) return false;

        return x.VerificationDate.Equals(y.VerificationDate) &&
            x.DeviceTypeNumber.Equals(y.DeviceTypeNumber) &&
            x.DeviceSerial.Equals(y.DeviceSerial);
    }

    public int GetHashCode([DisallowNull] T obj)
    {
        return HashCode.Combine(obj.VerificationDate, obj.DeviceTypeNumber, obj.DeviceSerial);
    }
}
