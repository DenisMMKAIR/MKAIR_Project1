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
        var vmIds = items.SelectMany(item => item.VerificationMethods!).Select(vm => vm.Id).ToArray();
        var dbVerificationMethods = await _database.VerificationMethods
            .Where(vm => vmIds.Contains(vm.Id))
            .ToArrayAsync();

        foreach (var item in items)
        {
            // TODO: Possible large file upload. Add by id only
            item.VerificationMethods = item.VerificationMethods!
                .SelectMany(vm => dbVerificationMethods.Where(dbVM => vm.Id == dbVM.Id))
                .ToList();
        }

        return await base.ExecuteAsync(items);
    }
}

public class ProtocolTemplateUniqComparer : IEqualityComparer<ProtocolTemplate>
{
    public bool Equals(ProtocolTemplate? x, ProtocolTemplate? y)
    {
        if (x == null || y == null) return false;

        return x.DeviceTypeNumbers.Any(y.DeviceTypeNumbers.Contains);
    }

    public int GetHashCode([DisallowNull] ProtocolTemplate obj)
    {
        var hashCode = new HashCode();
        foreach (var deviceTypeNumber in obj.DeviceTypeNumbers.Order())
        {
            hashCode.Add(deviceTypeNumber);
        }
        return hashCode.ToHashCode();
    }
}
