using Mapster;
using ProjApp.Database.Entities;

namespace ProjApp.Mapping;

public class ProtocolTemplateDTO : IRegister
{
    public required Guid Id { get; init; }
    public required string DeviceTypeNumber { get; init; }
    public required string Group { get; init; }
    public required IDictionary<string, string> Checkups { get; init; }
    public required IDictionary<string, object> Values { get; init; }
    public required IReadOnlyList<ProtocolVerificationMethodDTO> VerificationMethods { get; init; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProtocolTemplate, ProtocolTemplateDTO>()
            .Map(dest => dest.VerificationMethods,
                 src => src.VerificationMethods.Adapt<IReadOnlyList<ProtocolVerificationMethodDTO>>());
    }
}

public record ProtocolVerificationMethodDTO : IRegister
{
    public required Guid Id { get; init; }
    public required string Description { get; init; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<VerificationMethod, ProtocolVerificationMethodDTO>();
    }
}
