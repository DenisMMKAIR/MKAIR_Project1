using ProjApp.Database.Entities;

namespace WebAPI.Controllers.Requests;

public record AddProtocolTemplateRequest(
    string DeviceTypeNumber,
    string Group,
    IReadOnlyList<ProtocolCheckup> Checkups,
    IDictionary<string, object> Values,
    IReadOnlyList<VerificationMethod> VerificationMethods
)
{
    public static ProtocolTemplate ToProtocolTemplate(AddProtocolTemplateRequest request)
    {
        return new ProtocolTemplate
        {
            DeviceTypeNumber = request.DeviceTypeNumber,
            Group = request.Group,
            Checkups = request.Checkups,
            Values = request.Values,
            VerificationMethods = request.VerificationMethods
        };
    }
}
