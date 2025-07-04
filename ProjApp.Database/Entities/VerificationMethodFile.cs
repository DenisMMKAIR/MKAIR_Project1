namespace ProjApp.Database.Entities;

public class VerificationMethodFile : DatabaseEntity
{
    public required string FileName { get; set; }
    public required string Mimetype { get; set; }
    public required byte[] Content { get; set; }
}
