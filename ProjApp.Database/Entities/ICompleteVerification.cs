using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Database.Entities;

public interface ICompleteVerification : IVerificationBase
{
    // string DeviceTypeNumber { get; set; } - inherited
    // string DeviceSerial { get; set; } - inherited
    // DateOnly VerificationDate { get; set; } - inherited
    // IReadOnlyList<string> VerificationTypeNames { get; set; }
    string VerificationTypeNum { get; set; }
    string Owner { get; set; }
    ulong OwnerInn { get; set; }
    string Worker { get; set; }
    DeviceLocation Location { get; set; }
    string AdditionalInfo { get; set; }
    string Pressure { get; set; }
    double Temperature { get; set; }
    double Humidity { get; set; }
    Dictionary<string, object> Values { get; set; }
    string FilePath { get; set; }

    // Navigation properties
    ProtocolTemplate? ProtocolTemplate { get; set; }
    // TODO: CHeck EF Handles proper tables creation
    ICollection<VerificationMethod>? VerificationMethods { get; set; }
    // Device? Device { get; set; } - inherited
    // ICollection<Etalon>? Etalons { get; set; } - inherited
}

public class SuccessCompleteVerification : DatabaseEntity, ICompleteVerification
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }
    public required IReadOnlyList<string> VerificationTypeNames { get; set; }
    public required string VerificationTypeNum { get; set; }
    public required string Owner { get; set; }
    public required ulong OwnerInn { get; set; }
    public required string Worker { get; set; }
    public required DeviceLocation Location { get; set; }
    public required string AdditionalInfo { get; set; }
    public required string Pressure { get; set; }
    public required double Temperature { get; set; }
    public required double Humidity { get; set; }
    public required Dictionary<string, object> Values { get; set; }
    public required string FilePath { get; set; }

    // Navigation properties
    public ProtocolTemplate? ProtocolTemplate { get; set; }
    public ICollection<VerificationMethod>? VerificationMethods { get; set; }
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}

public class FailedCompleteVerification : DatabaseEntity, ICompleteVerification
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required string FailedDocNumber { get; set; }
    public required IReadOnlyList<string> VerificationTypeNames { get; set; }
    public required string VerificationTypeNum { get; set; }
    public required string Owner { get; set; }
    public required ulong OwnerInn { get; set; }
    public required string Worker { get; set; }
    public required DeviceLocation Location { get; set; }
    public required string AdditionalInfo { get; set; }
    public required string Pressure { get; set; }
    public required double Temperature { get; set; }
    public required double Humidity { get; set; }
    public required Dictionary<string, object> Values { get; set; }
    // TODO: Set Application referenced file path(Content path for example)
    public required string FilePath { get; set; }

    // Navigation properties
    public ProtocolTemplate? ProtocolTemplate { get; set; }
    public ICollection<VerificationMethod>? VerificationMethods { get; set; }
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}