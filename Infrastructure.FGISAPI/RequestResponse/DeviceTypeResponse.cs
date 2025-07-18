namespace Infrastructure.FGISAPI.RequestResponse;

internal class DeviceTypeResponse
{
  public required GeneralClass General { get; init; }
  public IReadOnlyList<MethodClass>? Meth { get; init; }
  public IReadOnlyList<SpecClass>? Spec { get; init; }
  public IReadOnlyList<ManufacturerClass>? Manufacturer { get; init; }

  public class GeneralClass
  {
    public required Guid MIT_UUID { get; init; }
    public required string Number { get; init; }
    public required string Title { get; init; }
    public required IReadOnlyList<string> Notation { get; init; }
  }

  public class MethodClass
  {
    public string? Title { get; init; }
    public string? Doc_URL { get; init; }
  }

  public class SpecClass
  {
    public string? Doc_URL { get; init; }
  }

  public class ManufacturerClass
  {
    public string? Title { get; init; }
  }
}
