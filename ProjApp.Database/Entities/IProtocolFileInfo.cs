using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Database.Entities;

public interface IProtocolFileInfo
{
    string DeviceSerial { get; }
    DeviceLocation Location { get; }
    VerificationGroup VerificationGroup { get; }
    DateOnly VerificationDate { get; }
    DateOnly VerifiedUntilDate { get; }
}
