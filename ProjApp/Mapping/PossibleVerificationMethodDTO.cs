using ProjApp.Database.Entities;

namespace ProjApp.Mapping;

public record PossibleVerificationMethodDTO(string Name, string DeviceTypeNumber,string DeviceTypeTitle)
{
    public static PossibleVerificationMethodDTO MapTo(IInitialVerification vrf)
    {
        return new(vrf.VerificationTypeName, vrf.DeviceTypeNumber, vrf.Device!.DeviceType!.Title);
    }
}
