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
        var vmIds = items.SelectMany(pt => pt.VerificationMethods!.Select(vm => vm.Id)).ToArray();
        var dbVerificationMethods = await _database.VerificationMethods.Where(vm => vmIds.Contains(vm.Id)).ToArrayAsync();

        foreach (var item in items)
        {
            item.VerificationMethods = [.. item.VerificationMethods!.Select(vm => dbVerificationMethods.Single(d => vm.Id == d.Id))];
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
