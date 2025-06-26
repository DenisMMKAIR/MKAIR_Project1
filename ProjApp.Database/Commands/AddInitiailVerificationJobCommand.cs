using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddInitialVerificationJobCommand : AddWithUniqConstraintCommand<InitialVerificationJob>
{
    public AddInitialVerificationJobCommand(ILogger<AddWithUniqConstraintCommand<InitialVerificationJob>> logger,
         ProjDatabase db) :
         base(logger, db, new InitialVerificationJobUniqComparer())
    { }
}

public class InitialVerificationJobUniqComparer : IEqualityComparer<InitialVerificationJob>
{
    public bool Equals(InitialVerificationJob? x, InitialVerificationJob? y)
    {
        if (x == null || y == null) return false;

        return x.Date == y.Date;
    }

    public int GetHashCode([DisallowNull] InitialVerificationJob obj)
    {
        return HashCode.Combine(obj.Date);
    }
}