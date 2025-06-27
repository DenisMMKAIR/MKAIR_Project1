namespace ProjApp.Database.Entities;

public class InitialVerification : DatabaseEntity
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required string Owner { get; set; }
    public required string AdditionalInfo { get; set; }
    public required string VerificationTypeName { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }
    public Device? Device { get; set; }
    public IReadOnlyList<Etalon>? Etalons { get; set; }
}
