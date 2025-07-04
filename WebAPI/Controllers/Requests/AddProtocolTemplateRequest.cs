using Mapster;
using ProjApp.Database.Entities;

namespace WebAPI.Controllers.Requests;

public class AddProtocolTemplateRequest : IRegister
{
    public required string DeviceTypeNumber { get; init; }
    public required string Group { get; init; }
    public required IDictionary<string, string> Checkups { get; init; }
    public required IDictionary<string, object> Values { get; init; }
    public required IReadOnlyList<Guid> VerificationMethodsIds { get; init; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AddProtocolTemplateRequest, ProtocolTemplate>()
            .Map(dest => dest.VerificationMethods, src => src.VerificationMethodsIds.Select(ToVerificationMethod))
            .Map(dest => dest.DeviceTypeNumbers, src => new string[] { src.DeviceTypeNumber });
    }

    private static VerificationMethod ToVerificationMethod(Guid id) => new()
    {
        Id = id,
        Aliases = [],
        Description = "",
    };
}
