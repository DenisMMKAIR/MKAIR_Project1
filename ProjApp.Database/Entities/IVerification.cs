using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Database.Entities;

public interface IVerification
{
    string DeviceTypeNumber { get; set; }
    string DeviceSerial { get; set; }
    string Owner { get; set; }
    IReadOnlyList<string> VerificationTypeNames { get; set; }
    DateOnly VerificationDate { get; set; }
    string VerificationTypeNum { get; set; }
    ulong OwnerInn { get; set; }
    string Worker { get; set; }
    DeviceLocation Location { get; set; }
    string AdditionalInfo { get; set; }
    string Pressure { get; set; }
    double Temperature { get; set; }
    double Humidity { get; set; }

    // Navigation properties
    Device? Device { get; set; }
    ICollection<Etalon>? Etalons { get; set; }
}

public class SuccessVerification : DatabaseEntity, IVerification
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required string Owner { get; set; }
    public required IReadOnlyList<string> VerificationTypeNames { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }
    public required string VerificationTypeNum { get; set; }
    public required ulong OwnerInn { get; set; }
    public required string Worker { get; set; }
    public required DeviceLocation Location { get; set; }
    public required string AdditionalInfo { get; set; }
    public required string Pressure { get; set; }
    public required double Temperature { get; set; }
    public required double Humidity { get; set; }

    // Navigation properties
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}

public class FailedVerification : DatabaseEntity, IVerification
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required string Owner { get; set; }
    public required IReadOnlyList<string> VerificationTypeNames { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required string FailedDocNumber { get; set; }
    public required string VerificationTypeNum { get; set; }
    public required ulong OwnerInn { get; set; }
    public required string Worker { get; set; }
    public required DeviceLocation Location { get; set; }
    public required string AdditionalInfo { get; set; }
    public required string Pressure { get; set; }
    public required double Temperature { get; set; }
    public required double Humidity { get; set; }

    // Navigation properties
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}
