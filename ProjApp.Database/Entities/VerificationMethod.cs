namespace ProjApp.Database.Entities;

public class VerificationMethod : DatabaseEntity
{
    public required IReadOnlyList<string> Aliases { get; set; }
    public required string Description { get; set; }

    // Navigation properties
    public ICollection<VerificationMethodFile>? VerificationMethodFiles { get; set; }
    public ICollection<ProtocolTemplate>? ProtocolTemplates { get; set; }
}
