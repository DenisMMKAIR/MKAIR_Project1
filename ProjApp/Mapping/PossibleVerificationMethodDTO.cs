using Mapster;
using ProjApp.Database.Entities;
using ProjApp.Database.SupportTypes;

namespace ProjApp.Mapping;

public record PossibleVrfMethodDTO(
    string DeviceTypeNumber,
    string DeviceTypeInfo,
    IReadOnlyList<string> MethodUrls,
    IReadOnlyList<string> SpecUrls,
    IReadOnlyList<PossibleVrfMethodAliasGroupDTO> AliasGroups
);

public record PossibleVrfMethodAliasGroupDTO(
    IReadOnlyList<PossibleVrfMethodAliasDTO> Aliases,
    IReadOnlyList<string> Modifications,
    IReadOnlyList<YearMonth> Dates
);

public record PossibleVrfMethodAliasDTO(
    bool Exists,
    string Alias
);

[Obsolete("Delete this")]
public class PossibleVerificationMethodDTO : IRegister
{
    public required string DeviceTypeNumber { get; init; }
    public required string DeviceTypeInfo { get; init; }
    public Guid? VerificationMethodId { get; init; }
    public required IReadOnlyList<string> DeviceModifications { get; init; }
    public required IReadOnlyList<string> VerificationTypeNames { get; init; }
    public required IReadOnlyList<YearMonth> Dates { get; init; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<IEnumerable<PossibleVerificationMethodPreSelectDTO>, PossibleVerificationMethodDTO>()
            .Map(dest => dest.DeviceTypeNumber, src => src.First().DeviceTypeNumber)
            .Map(dest => dest.DeviceTypeInfo, src => src.First().DeviceTypeInfo)
            .Map(dest => dest.VerificationMethodId, src => src.First().VerificationMethodId)
            .Map(dest => dest.DeviceModifications, src => src.Select(d => d.DeviceModification).Distinct().Order())
            .Map(dest => dest.VerificationTypeNames, src => src.Select(dto => dto.VerificationTypeName).Distinct().OrderBy(a => a.Length))
            .Map(dest => dest.Dates, src => src.Select(dto => (YearMonth)dto.VerificationDate).Distinct().Order());
    }
}

[Obsolete("Delete this")]
public class PossibleVerificationMethodPreSelectDTO : IRegister
{
    public required string DeviceTypeNumber { get; init; }
    public required string DeviceTypeInfo { get; init; }
    public required string DeviceModification { get; init; }
    public required string VerificationTypeName { get; init; }
    public required DateOnly VerificationDate { get; init; }
    public Guid? VerificationMethodId { get; init; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<IVerificationBase, PossibleVerificationMethodPreSelectDTO>()
            .Map(dest => dest.DeviceTypeInfo, src => src.Device!.DeviceType!.Title + " " + src.Device!.DeviceType!.Notation)
            .Map(dest => dest.DeviceModification, src => src.Device!.Modification)
            .Map(dest => dest.VerificationMethodId, src => src.VerificationMethod!.Id);
    }
}
