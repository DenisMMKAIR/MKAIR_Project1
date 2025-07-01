using ProjApp.Database.Entities;

namespace ProjApp.Mapping;

public record VerificationMethodDTO(
    Guid Id,
    IReadOnlyList<string> Aliases,
    string Description,
    string FileName)
{
    public static VerificationMethodDTO MapTo(VerificationMethod vrf)
    {
        return new(
            vrf.Id,
            vrf.Aliases,
            vrf.Description,
            vrf.FileName);
    }
}
