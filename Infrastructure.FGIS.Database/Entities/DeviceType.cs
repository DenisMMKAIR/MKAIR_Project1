namespace Infrastructure.FGIS.Database.Entities;

public class DeviceType
{
  public required Guid Id { get; init; }
  public required string Number { get; init; }
  public required string Title { get; init; }
  public required IReadOnlyList<string> Notation { get; init; }
  public IReadOnlyList<MethodClass>? Meth { get; init; }
  public IReadOnlyList<SpecClass>? Spec { get; init; }
  public IReadOnlyList<ManufacturerClass>? Manufacturer { get; init; }

  public class MethodClass
  {
    public required string Title { get; init; }
    public required string DocUrl { get; init; }
  }

  public class SpecClass
  {
    public required string DocUrl { get; init; }
  }

  public class ManufacturerClass
  {
    public required string Title { get; init; }
  }
}
