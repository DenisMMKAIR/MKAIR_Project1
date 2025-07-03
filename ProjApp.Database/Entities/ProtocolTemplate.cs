
namespace ProjApp.Database.Entities;

public class ProtocolTemplate : DatabaseEntity
{
    public required string DeviceTypeNumber { get; set; }
    public required string Group { get; set; }
    public required IDictionary<string, string> Checkups { get; set; }
    public required IDictionary<string, object> Values { get; set; }

    // Navigation properties
    public ICollection<VerificationMethod>? VerificationMethods { get; set; }
    public ICollection<CompleteVerificationSuccess>? CompleteSuccessVerifications { get; set; }
    public ICollection<CompleteVerificationFail>? CompleteFailVerifications { get; set; }
}

public interface ICompleteVerification
{
    public Guid Id { get; set; }
}

public class CompleteVerificationSuccess : DatabaseEntity, ICompleteVerification { }

public class CompleteVerificationFail : DatabaseEntity, ICompleteVerification { }
