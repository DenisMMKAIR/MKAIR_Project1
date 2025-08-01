using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Mapping;

public class Manometr1VerificationDto
{
    public required Guid Id { get; set; }
    public required string ProtocolNumber { get; set; }
    public required string DeviceTypeName { get; set; }
    public required string DeviceModification { get; set; }
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required uint ManufactureYear { get; set; }
    public required string Owner { get; set; }
    public required ulong OwnerINN { get; set; }
    public required string VerificationsInfo { get; set; }
    public required string EtalonsInfo { get; set; }
    public required double Temperature { get; set; }
    public required double Humidity { get; set; }
    public required string Pressure { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required string Worker { get; set; }

    // Support values
    public required VerificationGroup VerificationGroup { get; set; }
    public required DeviceLocation Location { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }

    // Table values
    public required double MeasurementMin { get; set; }
    public required double MeasurementMax { get; set; }
    public required string MeasurementUnit { get; set; }
    public required double ValidError { get; set; }

    public required IReadOnlyList<IReadOnlyList<double>> DeviceValues { get; set; }
    public required IReadOnlyList<IReadOnlyList<double>> EtalonValues { get; set; }
    public required IReadOnlyList<IReadOnlyList<double>> ActualError { get; set; }
    public required IReadOnlyList<double> ActualVariation { get; set; }
}

public static class Manometr1VerificationDtoExtensions
{
    public static IQueryable<Manometr1VerificationDto> ToDto(this IQueryable<Manometr1Verification> vrfs)
    {
        return vrfs.Select(vrf => new Manometr1VerificationDto
        {
            Id = vrf.Id,
            ProtocolNumber = vrf.ProtocolNumber,
            DeviceTypeName = $"{vrf.Device!.DeviceType!.Title} {vrf.Device.DeviceType.Notation}",
            DeviceModification = vrf.Device.Modification,
            DeviceTypeNumber = vrf.DeviceTypeNumber,
            DeviceSerial = vrf.DeviceSerial,
            ManufactureYear = vrf.Device.ManufacturedYear,
            Owner = vrf.Owner!.Name,
            OwnerINN = vrf.Owner!.INN,
            VerificationsInfo = vrf.VerificationMethod!.Description,
            EtalonsInfo = string.Join("; ", vrf.Etalons!.Select(e => e.Number)),
            Temperature = vrf.Temperature,
            Humidity = vrf.Humidity,
            Pressure = vrf.Pressure,
            VerificationDate = vrf.VerificationDate,
            Worker = vrf.Worker,
            VerificationGroup = vrf.VerificationGroup,
            Location = vrf.Location,
            VerifiedUntilDate = vrf.VerifiedUntilDate,
            MeasurementMin = vrf.MeasurementMin,
            MeasurementMax = vrf.MeasurementMax,
            MeasurementUnit = vrf.MeasurementUnit,
            ValidError = vrf.ValidError,
            DeviceValues = vrf.DeviceValues,
            EtalonValues = vrf.EtalonValues,
            ActualError = vrf.ActualError,
            ActualVariation = vrf.ActualVariation
        });
    }
}