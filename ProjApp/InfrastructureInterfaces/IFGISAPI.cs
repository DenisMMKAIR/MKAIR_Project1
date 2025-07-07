using ProjApp.Database.Entities;
using ProjApp.Database.SupportTypes;

namespace ProjApp.InfrastructureInterfaces;

public interface IFGISAPI
{
    Task<IReadOnlyList<DeviceType>> GetDeviceTypesAsync(IReadOnlyList<string> deviceNumbers);
    Task<(IReadOnlyList<SuccessInitialVerification>, IReadOnlyList<FailedInitialVerification>)> GetInitialVerifications(YearMonth date);
}