namespace ProjApp.Database.Entities;

public class VerificationMethod : DatabaseEntity
{
    public required string Description { get; set; }
    public required IReadOnlyList<string> Checkups { get; set; }
    public IReadOnlyList<VerificationMethodAlias>? Aliases { get; set; }
    public IReadOnlyList<Protocol>? Protocols { get; set; }
}
