
namespace ProjApp.Database.Entities;

public class ProtocolTemplate : DatabaseEntity
{
    public required IReadOnlyList<string> DeviceTypeNumbers { get; set; }
    public required string Group { get; set; }
    public required IDictionary<string, string> Checkups { get; set; }
    public required IDictionary<string, object> Values { get; set; }

    // Navigation properties
    public ICollection<VerificationMethod>? VerificationMethods { get; set; }
    public ICollection<InitialVerification>? CompleteSuccessVerifications { get; set; }
    public ICollection<InitialVerificationFailed>? CompleteFailVerifications { get; set; }
}
