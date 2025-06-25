namespace ProjApp.Database.Entities;

public class VerificationMethod : DatabaseEntity
{
    public IList<DeviceType>? DeviceTypes { get; set; }
    public IList<VerificationType>? VerificationTypes { get; set; }
}

public class VerificationType : DatabaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required IDictionary<string, string> Checkups { get; set; }
}