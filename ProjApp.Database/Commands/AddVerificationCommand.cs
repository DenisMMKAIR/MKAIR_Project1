using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddVerificationCommand<T> : AddWithUniqConstraintCommand<T> where T : DatabaseEntity, IVerification
{
    public AddVerificationCommand(
        ILogger<AddVerificationCommand<T>> logger,
        ProjDatabase db)
        : base(logger, db, new VerificationUniqComparer())
    {
    }
}
