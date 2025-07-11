using Mapster;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace WebAPI.Controllers.Requests;

public class AddProtocolTemplateRequest : IRegister
{
    public required VerificationGroup VerificationGroup { get; set; }
    public required ProtocolGroup ProtocolGroup { get; set; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AddProtocolTemplateRequest, ProtocolTemplate>();
    }
}
