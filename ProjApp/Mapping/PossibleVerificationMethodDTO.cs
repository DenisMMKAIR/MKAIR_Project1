using Mapster;
using ProjApp.Database.Entities;

namespace ProjApp.Mapping;

public class PossibleVerificationMethodDTO
    : IRegister
{
    public required string Name { get; set; }
    public required string DeviceTypeTitle { get; init; }
    public required string DeviceTypeNumber { get; init; }
    
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<IInitialVerification, PossibleVerificationMethodDTO>()
            .Map(dest => dest.Name, src => src.VerificationTypeName)
            .Map(dest => dest.DeviceTypeTitle, src => src.Device!.DeviceType!.Title);
    }
}
