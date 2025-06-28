using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddInitialVerificationCommand : AddWithUniqConstraintCommand<InitialVerification>
{
    private readonly AddDeviceTypeCommand _addDeviceTypeCommand;
    private readonly AddDeviceCommand _addDeviceCommand;
    private readonly AddEtalonCommand _addEtalonCommand;

    public AddInitialVerificationCommand(ILogger<AddInitialVerificationCommand> logger,
         ProjDatabase db,
         AddDeviceTypeCommand addDeviceTypeCommand,
         AddDeviceCommand addDeviceCommand,
         AddEtalonCommand addEtalonCommand) :
         base(logger, db, new InitialVerificationUniqComparer())
    {
        _addDeviceTypeCommand = addDeviceTypeCommand;
        _addDeviceCommand = addDeviceCommand;
        _addEtalonCommand = addEtalonCommand;
    }

    public override async Task<Result> ExecuteAsync(IReadOnlyList<InitialVerification> items)
    {
        foreach (var item in items)
        {
            if (item.Device == null) return Result.Failed("Устройство не задано");
            if (item.Device.DeviceType == null) return Result.Failed("Тип устройства не задан");
            if (item.Etalons == null) return Result.Failed("Эталоны не заданы");

            var addDeviceTypeResult = await _addDeviceTypeCommand.ExecuteAsync(item.Device.DeviceType);
            if (addDeviceTypeResult.Error != null) return Result.Failed(addDeviceTypeResult.Error);
            item.Device.DeviceType = addDeviceTypeResult.SavedItems!.Single();

            var addDeviceResult = await _addDeviceCommand.ExecuteAsync(item.Device);
            if (addDeviceResult.Error != null) return Result.Failed(addDeviceResult.Error);
            item.Device = addDeviceResult.SavedItems!.Single();

            var addEtalonResult = await _addEtalonCommand.ExecuteAsync(item.Etalons);
            if (addEtalonResult.Error != null) return Result.Failed(addEtalonResult.Error);
            item.Etalons = addEtalonResult.SavedItems!;
        }
        return await base.ExecuteAsync(items);
    }
}

public class InitialVerificationUniqComparer : IEqualityComparer<InitialVerification>
{
    public bool Equals(InitialVerification? x, InitialVerification? y)
    {
        if (x == null || y == null) return false;

        return x.VerificationDate.Equals(y.VerificationDate) &&
            x.DeviceTypeNumber.Equals(y.DeviceTypeNumber) &&
            x.DeviceSerial.Equals(y.DeviceSerial);
    }

    public int GetHashCode([DisallowNull] InitialVerification obj)
    {
        return HashCode.Combine(obj.VerificationDate, obj.DeviceTypeNumber, obj.DeviceSerial);
    }
}
