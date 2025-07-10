using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Database.Entities;

public interface IInitialVerification : IVerificationBase
{
    // string DeviceTypeNumber { get; set; } - inherited
    // string DeviceSerial { get; set; } - inherited
    // DateOnly VerificationDate { get; set; } - inherited
    string VerificationTypeName { get; set; }
    string Owner { get; set; }
    Dictionary<string, object> AdditionalInfo { get; set; }

    // Optional
    VerificationGroup? VerificationGroup { get; set; }
    string? ProtocolNumber { get; set; }
    ulong? OwnerINN { get; set; }
    string? Worker { get; set; }
    DeviceLocation? Location { get; set; }
    string? Pressure { get; set; }
    double? Temperature { get; set; }
    double? Humidity { get; set; }

    // Navigation properties
    // Device? Device { get; set; } - inherited
    // ICollection<Etalon>? Etalons { get; set; } - inherited
}

public class SuccessInitialVerification : DatabaseEntity, IInitialVerification
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required string Owner { get; set; }
    public required string VerificationTypeName { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }
    public required Dictionary<string, object> AdditionalInfo { get; set; } = [];

    // Optional
    public VerificationGroup? VerificationGroup { get; set; }
    public string? ProtocolNumber { get; set; }
    public ulong? OwnerINN { get; set; }
    public string? Worker { get; set; }
    public DeviceLocation? Location { get; set; }
    public string? Pressure { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }


    // Navigation properties
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}

public class FailedInitialVerification : DatabaseEntity, IInitialVerification
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required string Owner { get; set; }
    public required string VerificationTypeName { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required string FailedDocNumber { get; set; }
    public required Dictionary<string, object> AdditionalInfo { get; set; } = [];

    // Optional
    public VerificationGroup? VerificationGroup { get; set; }
    public string? ProtocolNumber { get; set; }
    public ulong? OwnerINN { get; set; }
    public string? Worker { get; set; }
    public DeviceLocation? Location { get; set; }
    public string? Pressure { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }

    // Navigation properties
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}
