using ProjApp.Database.Entities;

namespace WebAPI.Controllers.Requests;

public class AddDeviceTypeRequest
{
    public required string Number { get; init; }
    public required string Title { get; init; }
    public required string Notation { get; init; }
    public required uint MPI { get; init; }
    public required IReadOnlyList<string> MethodUrls { get; init; }
    public required IReadOnlyList<string> SpecUrls { get; init; }
    public required IReadOnlyList<string> Manufacturers { get; init; }

    public DeviceType ToDeviceType() => new()
    {
        Number = Number,
        Title = Title,
        Notation = Notation,
        MethodUrls = MethodUrls,
        SpecUrls = SpecUrls,
        Manufacturers = Manufacturers,
    };
}
