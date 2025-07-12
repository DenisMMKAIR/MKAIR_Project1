using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;
using ProjApp.Database.Normalizers;

namespace ProjApp.Database.Commands;

public class AddInitialVerificationCommand<T> : AddWithUniqConstraintCommand<T> where T : DatabaseEntity, IInitialVerification
{
    private readonly AddDeviceTypeCommand _addDeviceTypeCommand;
    private readonly AddDeviceCommand _addDeviceCommand;
    private readonly AddEtalonCommand _addEtalonCommand;
    private readonly ProjDatabase _database;

    public AddInitialVerificationCommand(ILogger<AddInitialVerificationCommand<T>> logger,
         ProjDatabase database,
         AddDeviceTypeCommand addDeviceTypeCommand,
         AddDeviceCommand addDeviceCommand,
         AddEtalonCommand addEtalonCommand) :
         base(logger, database, new VerificationUniqComparer())
    {
        _addDeviceTypeCommand = addDeviceTypeCommand;
        _addDeviceCommand = addDeviceCommand;
        _addEtalonCommand = addEtalonCommand;
        _database = database;
    }

    public override async Task<Result> ExecuteAsync(IReadOnlyList<T> items, IDbContextTransaction? parentTransaction = null)
    {
        var deviceTypeUniqComparer = new DeviceTypeUniqComparer();
        IReadOnlyList<DeviceType> uniqDeviceTypes = [.. items.Select(x => x.Device!.DeviceType!).Distinct(deviceTypeUniqComparer)];
        var savedDevicesTypes = await _addDeviceTypeCommand.ExecuteAsync(uniqDeviceTypes, parentTransaction);

        foreach (var device in items.Select(i => i.Device!))
        {
            device.DeviceType = savedDevicesTypes.Items!.Single(dt => deviceTypeUniqComparer.Equals(device.DeviceType!, dt));
        }

        var deviceUniqComparer = new DeviceUniqComparer();
        IReadOnlyList<Device> uniqDevices = [.. items.Select(x => x.Device!).Distinct(deviceUniqComparer)];
        var savedDevices = await _addDeviceCommand.ExecuteAsync(uniqDevices, parentTransaction);

        var etalonsUniqComparer = new EtalonUniqComparer();
        IReadOnlyList<Etalon> uniqEtalons = [.. items.SelectMany(x => x.Etalons!).Distinct(etalonsUniqComparer)];
        var savedEtalons = await _addEtalonCommand.ExecuteAsync(uniqEtalons, parentTransaction);

        // TODO: Possible problem. With this logic we wont return item if it already exists, unlike
        // another commands
        var vrfDB = _database.SuccessInitialVerifications
                         .AsEnumerable<IVerificationBase>()
                         .Union(_database.FailedInitialVerifications)
                         .Union(_database.SuccessVerifications)
                         .Union(_database.FailedVerifications)
                         .Union(_database.Manometr1Verifications);

        items = items.Except(vrfDB, VerificationUniqComparer.Instance).Cast<T>().ToArray();
        var nameNormalizer = new ComplexStringNormalizer();

        foreach (var item in items)
        {
            item.VerificationTypeName = nameNormalizer.Normalize(item.VerificationTypeName);
            item.Device = savedDevices.Items!.Single(d => deviceUniqComparer.Equals(item.Device!, d));
            item.Etalons = [.. item.Etalons!.Select(e => savedEtalons.Items!.Single(e2 => etalonsUniqComparer.Equals(e, e2)))];
        }

        return await base.ExecuteAsync(items, parentTransaction);
    }
}
