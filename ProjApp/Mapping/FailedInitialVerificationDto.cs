using Mapster;
using ProjApp.Database.Entities;

namespace ProjApp.Mapping;

public class FailedInitialVerificationDto : IRegister
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required string DeviceTypeInfo { get; set; }
    public required string VerificationTypeName { get; set; }
    public required string Owner { get; set; }
    public required IReadOnlyList<string> Etalons { get; set; }
    public required Guid Id { get; set; }
    public required string FailedDocNumber { get; set; }

    // Optional
    public string? AdditionalInfo { get; set; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<InitialVerificationFailed, FailedInitialVerificationDto>()
            .Map(dest => dest.DeviceTypeInfo, src => $"{src.Device!.DeviceType!.Title} {src.Device.DeviceType.Notation}")
            .Map(dest => dest.Etalons, src => src.Etalons!.Select(e => e.Number));
    }
}
