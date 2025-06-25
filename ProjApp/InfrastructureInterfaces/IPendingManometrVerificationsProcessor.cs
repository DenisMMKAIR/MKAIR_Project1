using ProjApp.Database;
using ProjApp.Database.Entities;

namespace ProjApp.InfrastructureInterfaces;

public interface IPendingManometrVerificationsProcessor
{
    IReadOnlyList<PendingManometrVerification> ReadVerificationFile(Stream fileStream, string fileName, string sheetName, string dataRange, DeviceLocation location);
}
