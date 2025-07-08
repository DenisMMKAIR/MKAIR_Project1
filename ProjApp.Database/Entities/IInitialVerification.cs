using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Database.Entities;

public interface IInitialVerification : IVerificationBase
{
    // public string DeviceTypeNumber { get; set; } - inherited
    // public string DeviceSerial { get; set; } - inherited
    // public DateOnly VerificationDate { get; set; } - inherited
    public IReadOnlyList<string> VerificationTypeNames { get; set; }
    public string Owner { get; set; }

    // Optional
    public string? VerificationTypeNum { get; set; }
    public ulong? OwnerInn { get; set; }
    public string? Worker { get; set; }
    public DeviceLocation? Location { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? Pressure { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }

    // Navigation properties
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}

public class SuccessInitialVerification : DatabaseEntity, IInitialVerification
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required string Owner { get; set; }
    public required IReadOnlyList<string> VerificationTypeNames { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }

    // Optional
    public string? VerificationTypeNum { get; set; }
    public ulong? OwnerInn { get; set; }
    public string? Worker { get; set; }
    public DeviceLocation? Location { get; set; }
    public string? AdditionalInfo { get; set; }
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
    public required IReadOnlyList<string> VerificationTypeNames { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required string FailedDocNumber { get; set; }

    // Optional
    public string? VerificationTypeNum { get; set; }
    public ulong? OwnerInn { get; set; }
    public string? Worker { get; set; }
    public DeviceLocation? Location { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? Pressure { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }

    // Navigation properties
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}
