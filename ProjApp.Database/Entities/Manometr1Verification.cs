using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Database.Entities;

public class Manometr1Verification : DatabaseEntity, IVerificationBase
{
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
    public required string VerificationVisualCheckup { get; set; }
    public required string VerificationResultCheckup { get; set; }
    public required string VerificationAccuracyCheckup { get; set; }
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

    // Navigation properties
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }

    public override string ToString()
        => $"[ProtocolNumber]{ProtocolNumber} [TypeNumber]{DeviceTypeNumber} [Serial]{DeviceSerial}";
}
