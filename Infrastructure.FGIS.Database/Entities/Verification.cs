namespace Infrastructure.FGIS.Database.Entities;

public record Verification(
    Guid Id,
    string OwnerName,
    DateOnly Date,
    DateOnly NextDate,
    string VerificationName,
    bool Applicable,
    string AdditionalInfo)
{
    public DeviceType? DeviceType { get; set; }
    public Device? Device { get; set; }
    public IReadOnlyList<Etalon>? Etalons { get; set; }
}