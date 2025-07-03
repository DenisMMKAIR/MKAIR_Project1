using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddProtocolTemplateCommand : AddWithUniqConstraintCommand<ProtocolTemplate>
{
    private readonly ProjDatabase _database;

    public AddProtocolTemplateCommand(ILogger<ProtocolTemplate> logger,
         ProjDatabase db) :
         base(logger, db, new ProtocolTemplateUniqComparer())
    {
        _database = db;
    }

    public override async Task<Result> ExecuteAsync(params IReadOnlyList<ProtocolTemplate> items)
    {
        var vmAliases = items.SelectMany(pt => pt.VerificationMethods!.SelectMany(vm => vm.Aliases)).ToArray();

        var dbVerificationMethods = await _database.VerificationMethods
            .Where(vm => vmAliases.Any(a => vm.Aliases.Any(va => a == va))).ToArrayAsync();

        foreach (var item in items)
        {
            item.VerificationMethods = item.VerificationMethods!
                .Select(vm => dbVerificationMethods.Single(dbVM =>
                    dbVM.Aliases.Any(dbA => item.VerificationMethods!.Any(iVM => iVM.Aliases.Contains(dbA)))))
                .ToArray();
        }

        return await base.ExecuteAsync(items);
    }
}

public class ProtocolTemplateUniqComparer : IEqualityComparer<ProtocolTemplate>
{
    public bool Equals(ProtocolTemplate? x, ProtocolTemplate? y)
    {
        if (x == null || y == null) return false;

        return x.DeviceTypeNumber == y.DeviceTypeNumber;
    }

    public int GetHashCode([DisallowNull] ProtocolTemplate obj)
    {
        return obj.DeviceTypeNumber.GetHashCode();
    }
}
