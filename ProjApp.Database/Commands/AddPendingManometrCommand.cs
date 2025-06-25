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
