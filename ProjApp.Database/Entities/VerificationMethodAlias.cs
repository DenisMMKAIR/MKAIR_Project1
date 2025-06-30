namespace ProjApp.Database.Entities;

public class VerificationMethodAlias : DatabaseEntity
{
    public required string Name { get; set; }
    public IReadOnlyList<VerificationMethod>? VerificationMethods { get; set; }
}
