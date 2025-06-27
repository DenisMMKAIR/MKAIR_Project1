namespace Infrastructure.FGIS.Database.Entities;

public record DeviceType(
    Guid Id,
    string DeviceTypeNumber,
    string DeviceTypeNotation,
    string DeviceTypeTitle);
