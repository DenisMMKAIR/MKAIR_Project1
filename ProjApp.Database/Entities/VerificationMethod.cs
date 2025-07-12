using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Database.Entities;

public class VerificationMethod : DatabaseEntity
{
    public required IReadOnlyList<string> Aliases { get; set; }
    public required string Description { get; set; }
    public required Dictionary<VerificationMethodCheckups, string> Checkups { get; set; }

    // Navigation properties
    public Guid? ProtocolTemplateId { get; set; }
    public ProtocolTemplate? ProtocolTemplate { get; set; }
    public ICollection<VerificationMethodFile>? VerificationMethodFiles { get; set; }
    public ICollection<DeviceType>? DeviceTypes { get; set; }
}
