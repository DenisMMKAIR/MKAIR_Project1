using Mapster;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Mapping;

public class InitialVerificationDto : IRegister
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required string DeviceTypeInfo { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }
    public required IReadOnlyList<string> VerificationTypeNames { get; set; }
    public required string Owner { get; set; }
    public required IReadOnlyList<string> Etalons { get; set; }
    public required Guid Id { get; set; }

    // Optional
    public string? VerificationTypeNum { get; set; }
    public uint? OwnerInn { get; set; }
    public string? Worker { get; set; }
    public DeviceLocation? Location { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? Pressure { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<SuccessInitialVerification, InitialVerificationDto>()
            .Map(dest => dest.DeviceTypeInfo, src => $"{src.Device!.DeviceType!.Title} {src.Device.DeviceType.Notation}")
            .Map(dest => dest.Etalons, src => src.Etalons!.Select(e => e.Number));
    }
}
