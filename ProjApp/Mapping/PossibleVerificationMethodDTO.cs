using ProjApp.Database.Entities;
using ProjApp.Normalizers;

namespace ProjApp.Mapping;

public record PossibleVerificationMethodDTO(string Name, string DeviceTypeNumber, string DeviceTypeTitle)
{
    private static readonly ComplexStringNormalizer _normalizer = new();
    public static PossibleVerificationMethodDTO MapTo(IInitialVerification vrf)
    {
        return new(_normalizer.Normalize(vrf.VerificationTypeName), vrf.DeviceTypeNumber, vrf.Device!.DeviceType!.Title);
    }
}
