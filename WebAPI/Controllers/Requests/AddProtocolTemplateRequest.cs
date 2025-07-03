using Mapster;
using ProjApp.Database.Entities;

namespace WebAPI.Controllers.Requests;

public class AddProtocolTemplateRequest : IRegister
{
    public required string DeviceTypeNumber { get; init; }
    public required string Group { get; init; }
    public required IDictionary<string, string> Checkups { get; init; }
    public required IDictionary<string, object> Values { get; init; }
    public required IReadOnlyList<IReadOnlyList<string>> VerificationMethodsAliases { get; init; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AddProtocolTemplateRequest, ProtocolTemplate>()
            .Map(dest => dest.VerificationMethods, src => src.VerificationMethodsAliases.Select(ToVerificationMethod));
    }

    private static VerificationMethod ToVerificationMethod(IReadOnlyList<string> aliases) => new()
    {
        Aliases = aliases,
        Description = "",
        FileContent = [],
        FileName = "",
    };
}
