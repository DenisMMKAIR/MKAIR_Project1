using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddInitialVerificationCommand : AddWithUniqConstraintCommand<InitialVerification>
{
    public AddInitialVerificationCommand(ILogger<AddWithUniqConstraintCommand<InitialVerification>> logger,
         ProjDatabase db) :
         base(logger, db, new InitialVerificationUniqComparer())
    { }
}

public class InitialVerificationUniqComparer : IEqualityComparer<InitialVerification>
{
    public bool Equals(InitialVerification? x, InitialVerification? y)
    {
        if (x == null || y == null) return false;

        return x.VerificationDate.Equals(y.VerificationDate) &&
            x.DeviceTypeNumber.Equals(y.DeviceTypeNumber) &&
            x.DeviceSerial.Equals(y.DeviceSerial);
    }

    public int GetHashCode([DisallowNull] InitialVerification obj)
    {
        return HashCode.Combine(obj.VerificationDate, obj.DeviceTypeNumber, obj.DeviceSerial);
    }
}
