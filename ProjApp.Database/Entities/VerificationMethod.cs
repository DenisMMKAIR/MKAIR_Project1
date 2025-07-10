namespace ProjApp.Database.Entities;

public class VerificationMethod : DatabaseEntity
{
    public required IReadOnlyList<string> Aliases { get; set; }
    public required string Description { get; set; }
    public required Dictionary<string, string> Checkups { get; set; }

    // Navigation properties
    public ICollection<VerificationMethodFile>? VerificationMethodFiles { get; set; }
}
