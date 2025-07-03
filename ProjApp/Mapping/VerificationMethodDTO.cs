using Mapster;
using ProjApp.Database.Entities;

namespace ProjApp.Mapping;

public record VerificationMethodDTO : IRegister
{
    public required Guid Id { get; init; }
    public required IReadOnlyList<string> Aliases { get; init; }
    public required string Description { get; init; }
    public required string FileName { get; init; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<VerificationMethod, VerificationMethodDTO>();
    }
}
