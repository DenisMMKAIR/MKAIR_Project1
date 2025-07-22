using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Database.Entities;

public class ProtocolTemplate : DatabaseEntity
{
    [Obsolete("Is this property needed?")]
    public required VerificationGroup VerificationGroup { get; set; }
    public required ProtocolGroup ProtocolGroup { get; set; }

    // Navigation properties
    public ICollection<VerificationMethod>? VerificationMethods { get; set; }
}
