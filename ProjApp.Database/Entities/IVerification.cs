using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Database.Entities;

public interface IVerification : IVerificationBase
{
    VerificationGroup VerificationGroup { get; set; }
    // string DeviceTypeNumber { get; set; } - inherited
    // string DeviceSerial { get; set; } - inherited
    string Owner { get; set; }
    string VerificationTypeName { get; set; }
    // DateOnly VerificationDate { get; set; } - inherited
    string ProtocolNumber { get; set; }
    ulong OwnerINN { get; set; }
    string Worker { get; set; }
    DeviceLocation Location { get; set; }
    string Pressure { get; set; }
    double Temperature { get; set; }
    double Humidity { get; set; }
    Dictionary<string, object> AdditionalInfo { get; set; }

    // Navigation properties
    // Device? Device { get; set; } - inherited
    // ICollection<Etalon>? Etalons { get; set; } - inherited
}

public class SuccessVerification : DatabaseEntity, IVerification
{
    public required VerificationGroup VerificationGroup { get; set; }
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required string Owner { get; set; }
    public required string VerificationTypeName { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }
    public required string ProtocolNumber { get; set; }
    public required ulong OwnerINN { get; set; }
    public required string Worker { get; set; }
    public required DeviceLocation Location { get; set; }
    public required string Pressure { get; set; }
    public required double Temperature { get; set; }
    public required double Humidity { get; set; }
    public required Dictionary<string, object> AdditionalInfo { get; set; }

    // Navigation properties
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}

public class FailedVerification : DatabaseEntity, IVerification
{
    public required VerificationGroup VerificationGroup { get; set; }
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required string Owner { get; set; }
    public required string VerificationTypeName { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required string FailedDocNumber { get; set; }
    public required string ProtocolNumber { get; set; }
    public required ulong OwnerINN { get; set; }
    public required string Worker { get; set; }
    public required DeviceLocation Location { get; set; }
    public required string Pressure { get; set; }
    public required double Temperature { get; set; }
    public required double Humidity { get; set; }
    public required Dictionary<string, object> AdditionalInfo { get; set; }

    // Navigation properties
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}
