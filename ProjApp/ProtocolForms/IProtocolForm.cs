using ProjApp.Database.Entities;

namespace ProjApp.ProtocolForms;

public interface IProtocolForm { }

public static class ProtocolFormExtensions
{
    public static IProtocolForm ToProtocolForm(this ICompleteVerification vrf)
    {
        return vrf switch
        {
            Davlenie1Verification davlenie => davlenie.ToDavlenieForm(),
            Manometr1Verification manometr => manometr.ToManometrForm(),
            _ => throw new NotImplementedException("Protocol form not implemented")
        };
    }
}
