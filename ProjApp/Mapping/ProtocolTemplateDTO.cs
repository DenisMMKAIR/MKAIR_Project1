using ProjApp.Database.Entities;

namespace ProjApp.Mapping;

public record ProtocolTemplateDTO(
    Guid Id,
    string DeviceTypeNumber,
    string Group,
    IReadOnlyList<ProtocolCheckup> Checkups,
    IDictionary<string, object> Values,
    IReadOnlyList<ProtocolVerificationMethodDTO> VerificationMethods)
{
    public static ProtocolTemplateDTO MapTo(ProtocolTemplate pt)
    {
        return new(
            pt.Id,
            pt.DeviceTypeNumber,
            pt.Group,
            pt.Checkups,
            pt.Values,
            [.. pt.VerificationMethods!.Select(ProtocolVerificationMethodDTO.MapTo)]);
    }
}

public record ProtocolVerificationMethodDTO(
    Guid Id,
    string Description
)
{
    public static ProtocolVerificationMethodDTO MapTo(VerificationMethod vrf)
    {
        throw new NotImplementedException();
    }
}
