using System.Diagnostics.CodeAnalysis;
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
         base(logger, database, new InitialVerificationUniqComparer<T>())
    {
        _addDeviceTypeCommand = addDeviceTypeCommand;
        _addDeviceCommand = addDeviceCommand;
        _addEtalonCommand = addEtalonCommand;
        _database = database;
    }

    public override async Task<Result> ExecuteAsync(params IReadOnlyList<T> items)
    {

        var deviceTypeUniqComparer = new DeviceTypeUniqComparer();
        IReadOnlyList<DeviceType> uniqDeviceTypes = [.. items.Select(x => x.Device!.DeviceType!).Distinct(deviceTypeUniqComparer)];
        var savedDevicesTypes = await _addDeviceTypeCommand.ExecuteAsync(uniqDeviceTypes);

        foreach (var device in items.Select(i => i.Device!))
        {
            device.DeviceType = savedDevicesTypes.Items!.Single(dt => deviceTypeUniqComparer.Equals(device.DeviceType!, dt));
        }

        var deviceUniqComparer = new DeviceUniqComparer();
        IReadOnlyList<Device> uniqDevices = [.. items.Select(x => x.Device!).Distinct(deviceUniqComparer)];
        var savedDevices = await _addDeviceCommand.ExecuteAsync(uniqDevices);

        var etalonsUniqComparer = new EtalonUniqComparer();
        IReadOnlyList<Etalon> uniqEtalons = [.. items.SelectMany(x => x.Etalons!).Distinct(etalonsUniqComparer)];
        var savedEtalons = await _addEtalonCommand.ExecuteAsync(uniqEtalons);

        var uniqComparer = new InitialVerificationUniqComparer<IVerificationBase>();

        // TODO: Possible problem. With this logic we wont return item if it already exists, unlike
        // another commands
        var vrfDB = _database.SuccessInitialVerifications
                         .AsEnumerable<IVerificationBase>()
                         .Union(_database.FailedInitialVerifications
                                         .AsEnumerable())
                         .Union(_database.SuccessVerifications
                                         .AsEnumerable())
                         .Union(_database.FailedVerifications
                                         .AsEnumerable());

        items = items.Except(vrfDB, uniqComparer).Cast<T>().ToArray();
        var nameNormalizer = new ComplexStringNormalizer();

        foreach (var item in items)
        {
            item.VerificationTypeNames = [.. item.VerificationTypeNames.Select(nameNormalizer.Normalize)];
            item.Device = savedDevices.Items!.Single(d => deviceUniqComparer.Equals(item.Device!, d));
            item.Etalons = [.. item.Etalons!.Select(e => savedEtalons.Items!.Single(e2 => etalonsUniqComparer.Equals(e, e2)))];
        }

        return await base.ExecuteAsync(items);
    }
}

public class InitialVerificationUniqComparer<T> : IEqualityComparer<T> where T : IVerificationBase
{
    public bool Equals(T? x, T? y)
    {
        if (x == null || y == null) return false;

        return x.VerificationDate.Equals(y.VerificationDate) &&
            x.DeviceTypeNumber.Equals(y.DeviceTypeNumber) &&
            x.DeviceSerial.Equals(y.DeviceSerial);
    }

    public int GetHashCode([DisallowNull] T obj)
    {
        return HashCode.Combine(obj.VerificationDate, obj.DeviceTypeNumber, obj.DeviceSerial);
    }
}
