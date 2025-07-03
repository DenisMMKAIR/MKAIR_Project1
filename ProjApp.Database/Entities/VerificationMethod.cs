namespace ProjApp.Database.Entities;

public class VerificationMethod : DatabaseEntity
{
    public required IReadOnlyList<string> Aliases { get; set; }
    public required string Description { get; set; }
    public required string FileName { get; set; }
    public required byte[] FileContent { get; set; }

    // Navigation properties
    public ICollection<ProtocolTemplate>? ProtocolTemplates { get; set; }
}
