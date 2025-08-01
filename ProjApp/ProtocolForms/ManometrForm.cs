using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace ProjApp.ProtocolForms;

public class ManometrForm : IProtocolForm
{
    public required string Address { get; init; }
    public required string ProtocolNumber { get; init; }
    public required string DeviceInfo { get; init; }
    public required string DeviceTypeNumber { get; init; }
    public required string DeviceSerial { get; init; }
    public required uint ManufactureYear { get; init; }
    public required string Owner { get; init; }
    public required ulong OwnerINN { get; init; }
    public required string VerificationsInfo { get; init; }
    public required string EtalonsInfo { get; init; }
    public required double Temperature { get; init; }
    public required double Humidity { get; init; }
    public required string Pressure { get; init; }
    public required IReadOnlyDictionary<string, string> Checkups { get; init; }
    public required string MeasurementUnit { get; init; }
    public required IReadOnlyList<IReadOnlyList<double>> DeviceValues { get; set; }
    public required IReadOnlyList<IReadOnlyList<double>> EtalonValues { get; set; }
    public required IReadOnlyList<IReadOnlyList<double>> ActualError { get; set; }
    public required double ValidError { get; set; }
    public required IReadOnlyList<double> ActualVariation { get; set; }
    public required DateOnly VerificationDate { get; init; }
    public required string Worker { get; init; }
}

public static class ManometrFormExtensions
{
    public static ManometrForm ToManometrForm(this Manometr1Verification vrf)
    {
        return new()
        {
            Address = MKAIRInfo.GetAddress(vrf.VerificationDate),
            ProtocolNumber = vrf.ProtocolNumber,
            DeviceInfo = $"{vrf.Device!.DeviceType!.Title} {vrf.Device!.DeviceType!.Notation}; {vrf.Device!.Modification}",
            DeviceTypeNumber = vrf.Device!.DeviceType!.Number,
            DeviceSerial = vrf.Device!.Serial,
            ManufactureYear = vrf.Device!.ManufacturedYear,
            Owner = vrf.Owner!.Name,
            OwnerINN = vrf.Owner!.INN,
            VerificationsInfo = $"{vrf.VerificationMethod!.Description}",
            EtalonsInfo = vrf.Etalons!.Select(e => e.FullInfo).Aggregate((a, c) => $"{a}; {c}"),
            Temperature = vrf.Temperature,
            Humidity = vrf.Humidity,
            Pressure = vrf.Pressure,
            Checkups = vrf.VerificationMethod.Checkups,
            MeasurementUnit = vrf.MeasurementUnit,
            DeviceValues = vrf.DeviceValues,
            EtalonValues = vrf.EtalonValues,
            ActualError = vrf.ActualError,
            ValidError = vrf.ValidError,
            ActualVariation = vrf.ActualVariation,
            VerificationDate = vrf.VerificationDate,
            Worker = vrf.Worker,
        };
    }
}