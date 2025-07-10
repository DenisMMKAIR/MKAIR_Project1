// using Mapster;
// using ProjApp.Database.Entities;
// using ProjApp.Database.EntitiesStatic;

// namespace WebAPI.Controllers.Requests;

// public class AddProtocolTemplateRequest : IRegister
// {
//     public required string DeviceTypeNumber { get; init; }
//     public required VerificationGroup Group { get; init; }
//     public required bool VerificationSuccess { get; init; }
//     public required Dictionary<string, string> Checkups { get; init; }
//     public required Dictionary<string, object> Values { get; init; }
//     public required IReadOnlyList<Guid> VerificationMethodsIds { get; init; }
//     public required string ProtocolForm { get; init; }

//     public void Register(TypeAdapterConfig config)
//     {
//         config.NewConfig<AddProtocolTemplateRequest, ProtocolTemplate>()
//             .Map(dest => dest.VerificationMethods, src => src.VerificationMethodsIds.Select(ToVerificationMethod))
//             .Map(dest => dest.DeviceTypeNumbers, src => new string[] { src.DeviceTypeNumber });
//     }

//     private static VerificationMethod ToVerificationMethod(Guid id) => new()
//     {
//         Id = id,
//         Aliases = [],
//         Description = "",
//     };
// }
