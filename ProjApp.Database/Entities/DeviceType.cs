namespace ProjApp.Database.Entities;

public class DeviceType : DatabaseEntity
{
    public required string Number { get; set; }
    public required string Title { get; set; }
    public required string Notation { get; set; }
    public IReadOnlyList<string>? MethodUrls { get; set; }
    public IReadOnlyList<string>? SpecUrls { get; set; }
    public IReadOnlyList<string>? Manufacturers { get; set; }

    // Navigation properties
}
