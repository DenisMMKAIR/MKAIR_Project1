namespace Infrastructure.FGIS.Database.Entities;

public class DeviceType
{
  public required Guid Id { get; init; }
  public required string Number { get; init; }
  public required string Title { get; init; }
  public required IReadOnlyList<string> Notation { get; init; }
  public IReadOnlyList<string>? MethUrls { get; init; }
  public IReadOnlyList<string>? SpecUrls { get; init; }
  public IReadOnlyList<string>? Manufacturers { get; init; }
}
