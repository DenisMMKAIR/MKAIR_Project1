namespace ProjApp.Database.Entities;

public class InitialVerification : DatabaseEntity, IInitialVerification
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required string Owner { get; set; }
    public required string VerificationTypeName { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }

    // Optional
    public string? AdditionalInfo { get; set; }

    // Navigation properties
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}

public interface IInitialVerification
{
    public string DeviceTypeNumber { get; set; }
    public string DeviceSerial { get; set; }
    public string Owner { get; set; }
    public string VerificationTypeName { get; set; }
    public DateOnly VerificationDate { get; set; }

    // Optional
    public string? AdditionalInfo { get; set; }

    // Navigation properties
    public Device? Device { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}
