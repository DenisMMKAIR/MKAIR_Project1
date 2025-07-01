namespace ProjApp.Database.Entities;

public class Protocol : DatabaseEntity
{
    public required string DeviceTypeNumber { get; set; }
    public required string Group { get; set; }
    public required IReadOnlyList<ProtocolCheckup> Checkups { get; set; }
    public required IReadOnlyList<VerificationMethod> VerificationMethods { get; set; }
}

public class ProtocolCheckup : DatabaseEntity
{
    public required string Name { get; set; }
    public required string Value { get; set; }
}
