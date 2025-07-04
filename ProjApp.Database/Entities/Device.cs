namespace ProjApp.Database.Entities;

public class Device : DatabaseEntity
{
    public required string DeviceTypeNumber { get; set; }
    public required string Serial { get; set; }
    public required uint ManufacturedYear { get; set; }
    public required string Modification { get; set; }

    // Navigation properties
    public DeviceType? DeviceType { get; set; }
}
