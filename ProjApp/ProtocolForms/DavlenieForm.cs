using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace ProjApp.ProtocolForms;

public class DavlenieForm
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
    public required string VisualCheckup { get; init; }
    public required string TestCheckup { get; init; }
    public required string AccuracyCalculation { get; init; }
    public required string MeasurementUnit { get; init; }
    public required IReadOnlyList<double> PressureInputs { get; init; }
    public required IReadOnlyList<double> EtalonValues { get; init; }
    public required IReadOnlyList<IReadOnlyList<double>> DeviceValues { get; init; }
    public required IReadOnlyList<IReadOnlyList<double>> ActualError { get; init; }
    public required double ValidError { get; init; }
    public required IReadOnlyList<double> Variations { get; init; }
    public required DateOnly VerificationDate { get; init; }
    public required string Worker { get; init; }
}

public static class DavlenieFormExtensions
{
    public static DavlenieForm ToDavlenieForm(this Davlenie1Verification vrf)
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
            VisualCheckup = vrf.VisualCheckup,
            TestCheckup = vrf.TestCheckup,
            AccuracyCalculation = vrf.AccuracyCalculation,
            MeasurementUnit = vrf.MeasurementUnit,
            PressureInputs = vrf.PressureInputs,
            EtalonValues = vrf.EtalonValues,
            DeviceValues = vrf.DeviceValues,
            ActualError = vrf.ActualError,
            ValidError = vrf.ValidError,
            Variations = vrf.Variations,
            VerificationDate = vrf.VerificationDate,
            Worker = vrf.Worker,
        };
    }
}