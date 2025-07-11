namespace ProjApp.Database.Entities;

public class DeviceType : DatabaseEntity
{
    public required string Number { get; set; }
    public required string Title { get; set; }
    public required string Notation { get; set; }

    // Navigation properties
    public Guid? VerificationMethodId { get; set; }
    public VerificationMethod? VerificationMethod { get; set; }
}
