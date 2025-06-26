using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddPendingManometrCommand : AddWithUniqConstraintCommand<PendingManometrVerification>
{
    public AddPendingManometrCommand(ILogger<AddWithUniqConstraintCommand<PendingManometrVerification>> logger,
         ProjDatabase db) :
         base(logger, db, new PendingManometrVerificationUniqComparer())
    { }
}

public class PendingManometrVerificationUniqComparer : IEqualityComparer<PendingManometrVerification>
{
    public bool Equals(PendingManometrVerification? x, PendingManometrVerification? y)
    {
        if (x == null || y == null) return false;

        return x.DeviceTypeNumber.Equals(y.DeviceTypeNumber) &&
               x.DeviceSerial.Equals(y.DeviceSerial) &&
               x.Date.Equals(y.Date) &&
               x.Location.Equals(y.Location);
    }

    public int GetHashCode([DisallowNull] PendingManometrVerification obj)
    {
        return HashCode.Combine(obj.DeviceTypeNumber, obj.DeviceSerial, obj.Date, obj.Location);
    }
}
