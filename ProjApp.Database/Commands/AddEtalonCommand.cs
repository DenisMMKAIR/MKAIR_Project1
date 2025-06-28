using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddEtalonCommand : AddWithUniqConstraintCommand<Etalon>
{
    public AddEtalonCommand(ILogger<AddWithUniqConstraintCommand<Etalon>> logger,
         ProjDatabase db) :
         base(logger, db, new EtalonUniqComparer())
    { }
}

public class EtalonUniqComparer : IEqualityComparer<Etalon>
{
    public bool Equals(Etalon? x, Etalon? y)
    {
        if (x == null || y == null) return false;

        return x.Number.Equals(y.Number) &&
               x.Date.Equals(y.Date);
    }

    public int GetHashCode([DisallowNull] Etalon obj)
    {
        return HashCode.Combine(obj.Number, obj.Date);
    }
}
