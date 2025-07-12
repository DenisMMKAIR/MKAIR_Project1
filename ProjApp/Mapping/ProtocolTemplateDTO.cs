using Mapster;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Mapping;

public class ProtocolTemplateDTO : IRegister
{
    public required Guid Id { get; init; }
    public required ProtocolGroup ProtocolGroup { get; init; }
    public required VerificationGroup VerificationGroup { get; init; }
    public required IReadOnlyList<string> VerificationMethodsAliases { get; init; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProtocolTemplate, ProtocolTemplateDTO>()
            .Map(dest => dest.VerificationMethodsAliases, src => src.VerificationMethods!
                .SelectMany(v => v.Aliases)
                .OrderBy(a => a.Length));
    }
}
