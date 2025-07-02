using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace ProjApp.InfrastructureInterfaces;

public interface IPendingManometrVerificationsProcessor
{
    IReadOnlyList<PendingManometrVerification> ReadVerificationFile(Stream fileStream, string fileName, string sheetName, string dataRange, DeviceLocation location);
}
