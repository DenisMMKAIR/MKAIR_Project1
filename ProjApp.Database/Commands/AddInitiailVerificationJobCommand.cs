using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddInitiailVerificationJobCommand : AddWithUniqConstraintCommand<InitiailVerificationJob>
{
    public AddInitiailVerificationJobCommand(ILogger<AddWithUniqConstraintCommand<InitiailVerificationJob>> logger,
         ProjDatabase db) :
         base(logger, db, new InitiailVerificationJobUniqComparer())
    { }
}
