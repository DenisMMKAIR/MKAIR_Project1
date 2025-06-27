namespace Infrastructure.FGIS.Database.Entities;

public record Device(
    Guid Id,
    string DeviceSerialNumber,
    uint DeviceManufacturedYear,
    string DeviceModification);
