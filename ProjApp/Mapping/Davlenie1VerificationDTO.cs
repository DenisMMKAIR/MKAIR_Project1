using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Mapping;

public class Davlenie1VerificationDTO
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
    public required string VisualCheckup { get; set; }
    public required string TestCheckup { get; set; }
    public required string AccuracyCalculation { get; set; }
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
    public required IReadOnlyList<double> PressureInputs { get; set; }
    public required IReadOnlyList<double> EtalonValues { get; set; }
    public required IReadOnlyList<IReadOnlyList<double>> DeviceValues { get; set; }
    public required IReadOnlyList<IReadOnlyList<double>> ActualError { get; set; }
    public required double ValidError { get; set; }
    public required IReadOnlyList<double> Variations { get; set; }
}

public static class Davlenie1VerificationDTOExtensions
{
    public static IQueryable<Davlenie1VerificationDTO> ToDTO(this IQueryable<Davlenie1Verification> query)
    {
        return query.Select(vrf => new Davlenie1VerificationDTO()
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
            VisualCheckup = vrf.VisualCheckup,
            TestCheckup = vrf.TestCheckup,
            AccuracyCalculation = vrf.AccuracyCalculation,
            VerificationDate = vrf.VerificationDate,
            Worker = vrf.Worker,
            VerificationGroup = vrf.VerificationGroup,
            Location = vrf.Location,
            VerifiedUntilDate = vrf.VerifiedUntilDate,
            MeasurementMin = vrf.MeasurementMin,
            MeasurementMax = vrf.MeasurementMax,
            MeasurementUnit = vrf.MeasurementUnit,
            PressureInputs = vrf.PressureInputs,
            EtalonValues = vrf.EtalonValues,
            DeviceValues = vrf.DeviceValues,
            ActualError = vrf.ActualError,
            ValidError = vrf.ValidError,
            Variations = vrf.Variations,
        });
    }
}