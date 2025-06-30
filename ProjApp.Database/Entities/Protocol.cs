namespace ProjApp.Database.Entities;

public class Protocol : DatabaseEntity
{
    public required string DeviceTypeNumber { get; set; }
    public required string Group { get; set; }
    public required IReadOnlyList<VerificationMethod> VerificationMethods { get; set; }
}
