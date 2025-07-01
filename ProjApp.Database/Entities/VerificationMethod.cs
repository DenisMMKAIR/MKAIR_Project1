namespace ProjApp.Database.Entities;

public class VerificationMethod : DatabaseEntity
{
    public required IReadOnlyList<VerificationMethodAlias> Aliases { get; set; }
    public required string Description { get; set; }
    public required string FileName { get; set; }
    public required byte[] FileContent { get; set; }
    public IReadOnlyList<Protocol>? Protocols { get; set; }
}
