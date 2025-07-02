
namespace ProjApp.Database.Entities;

public class ProtocolTemplate : DatabaseEntity
{
    public required string DeviceTypeNumber { get; set; }
    public required string Group { get; set; }
    public required IReadOnlyList<ProtocolCheckup> Checkups { get; set; }
    public required IDictionary<string, object> Values { get; set; }
    public IReadOnlyList<VerificationMethod>? VerificationMethods { get; set; }
    public IReadOnlyList<CompleteVerificationSuccess>? CompleteSuccessVerifications { get; set; }
    public IReadOnlyList<CompleteVerificationFail>? CompleteFailVerifications { get; set; }
}

public class ProtocolCheckup : DatabaseEntity
{
    public required string Name { get; set; }
    public required string Value { get; set; }
}

public interface ICompleteVerification
{
    public Guid Id { get; set; }
}

public class CompleteVerificationSuccess : DatabaseEntity, ICompleteVerification { }

public class CompleteVerificationFail : DatabaseEntity, ICompleteVerification { }
