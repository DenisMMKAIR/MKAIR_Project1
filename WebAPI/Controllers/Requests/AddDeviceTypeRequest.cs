using ProjApp.Database.Entities;

namespace WebAPI.Controllers.Requests;

public class AddDeviceTypeRequest
{
    public required string Number { get; set; }
    public required string Name { get; set; }
    public required string Notation { get; set; }

    public DeviceType ToDeviceType() => new()
    {
        Number = Number,
        Title = Name,
        Notation = Notation
    };
}