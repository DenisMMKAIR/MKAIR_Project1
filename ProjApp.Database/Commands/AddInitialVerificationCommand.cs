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
            x.Device!.DeviceType!.Number.Equals(y.Device!.DeviceType!.Number) &&
            x.Device!.Serial.Equals(y.Device!.Serial);
    }

    public int GetHashCode([DisallowNull] InitialVerification obj)
    {
        return HashCode.Combine(obj.VerificationDate, obj.Device!.DeviceType!.Number, obj.Device!.Serial);
    }
}
