using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public class AddDeviceTypeCommand : AddWithUniqConstraintCommand<DeviceType>
{
    public AddDeviceTypeCommand(ILogger<AddWithUniqConstraintCommand<DeviceType>> logger,
         ProjDatabase db) :
         base(logger, db, new DeviceTypeUniqComparer())
    { }
}
