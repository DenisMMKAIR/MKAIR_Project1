using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddEtalonCommand : AddWithUniqConstraintCommand<Etalon>
{
    public AddEtalonCommand(ILogger<AddEtalonCommand> logger,
         ProjDatabase db) :
         base(logger, db, new EtalonUniqComparer())
    { }
}

public class EtalonUniqComparer : IEqualityComparer<Etalon>
{
    public bool Equals(Etalon? x, Etalon? y)
    {
        if (x == null || y == null) return false;

        return x.Number == y.Number &&
               x.Date == y.Date;
    }

    public int GetHashCode([DisallowNull] Etalon obj)
    {
        return HashCode.Combine(obj.Number, obj.Date);
    }
}
