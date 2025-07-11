using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;

namespace ProjApp.Services;

public class AddProtocolTemplateCommand : AddWithUniqConstraintCommand<ProtocolTemplate>
{
    public AddProtocolTemplateCommand(ILogger<AddProtocolTemplateCommand> logger, ProjDatabase db)
        : base(logger, db, new ProtocolTemplateUniqComparer())
    {
    }
}

public class ProtocolTemplateUniqComparer : IEqualityComparer<ProtocolTemplate>
{
    public bool Equals(ProtocolTemplate? x, ProtocolTemplate? y)
    {
        if (x == null || y == null) return false;

        return x.ProtocolGroup == y.ProtocolGroup;
    }

    public int GetHashCode([DisallowNull] ProtocolTemplate obj)
    {
        return obj.ProtocolGroup.GetHashCode();
    }
}
