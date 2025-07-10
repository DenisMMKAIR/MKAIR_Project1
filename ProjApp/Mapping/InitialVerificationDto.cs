using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Mapster;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Mapping;

public class SuccessInitialVerificationDto : IRegister
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }
    public required string VerificationTypeName { get; set; }
    public required string Owner { get; set; }
    public required Guid Id { get; set; }
    public required string DeviceTypeInfo { get; set; }
    public required IReadOnlyList<string> Etalons { get; set; }
    public required string AdditionalInfo { get; set; }

    // Optional
    public VerificationGroup? VerificationGroup { get; set; }
    public string? ProtocolNumber { get; set; }
    public ulong? OwnerInn { get; set; }
    public string? Worker { get; set; }
    public DeviceLocation? Location { get; set; }
    public string? Pressure { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }

    public void Register(TypeAdapterConfig config)
    {
        var _options = new JsonSerializerOptions{Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)};

        config.NewConfig<SuccessInitialVerification, SuccessInitialVerificationDto>()
            .Map(dest => dest.DeviceTypeInfo, src => $"{src.Device!.DeviceType!.Title} {src.Device.DeviceType.Notation}")
            .Map(dest => dest.Etalons, src => src.Etalons!.Select(e => e.Number))
            .Map(dest => dest.AdditionalInfo, src => JsonSerializer.Serialize(src.AdditionalInfo, _options));
    }
}
