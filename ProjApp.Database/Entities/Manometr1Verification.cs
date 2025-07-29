using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Database.Entities;

[Obsolete("Remove checkups. get them from verification method")]
public class Manometr1Verification : DatabaseEntity, IVerificationBase, IProtocolFileInfo
{
    public required string ProtocolNumber { get; set; }
    public required double Temperature { get; set; }
    public required double Humidity { get; set; }
    public required string Pressure { get; set; }
    public required string VisualCheckup { get; set; }
    public required string TestCheckup { get; set; }
    public required string AccuracyCalculation { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required string Worker { get; set; }

    // Support values
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required VerificationGroup VerificationGroup { get; set; }
    public required DeviceLocation Location { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }
    public required string InitialVerificationName { get; set; }
    public required string OwnerInitialName { get; set; }

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
    public Guid? DeviceId { get; set; }
    public Device? Device { get; set; }
    public Guid? VerificationMethodId { get; set; }
    public VerificationMethod? VerificationMethod { get; set; }
    public Guid? OwnerId { get; set; }
    public Owner? Owner { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }

    public override string ToString()
        => $"[ProtocolNumber]{ProtocolNumber} [TypeNumber]{DeviceTypeNumber} [Serial]{DeviceSerial} [Date]{VerificationDate}";
}
