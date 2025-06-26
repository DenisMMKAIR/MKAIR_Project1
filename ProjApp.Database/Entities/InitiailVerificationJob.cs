using System.Diagnostics.CodeAnalysis;

namespace ProjApp.Database.Entities;

public class InitiailVerificationJob : DatabaseEntity
{
    public required string Date { get; set; }
    public required uint LoadedVerifications { get; set; }
}


public class InitiailVerificationJobUniqComparer : IEqualityComparer<InitiailVerificationJob>
{
    public bool Equals(InitiailVerificationJob? x, InitiailVerificationJob? y)
    {
        if (x == null || y == null) return false;

        return x.Date == y.Date;
    }

    public int GetHashCode([DisallowNull] InitiailVerificationJob obj)
    {
        return HashCode.Combine(obj.Date);
    }
}
