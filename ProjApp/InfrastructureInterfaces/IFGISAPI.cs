using ProjApp.Database.Entities;

namespace ProjApp.InfrastructureInterfaces;

public interface IFGISAPI
{
    Task<IReadOnlyList<DeviceType>> GetDeviceTypesAsync(IReadOnlyList<string> deviceNumbers);
    Task<IReadOnlyList<InitialVerification>> GetInitialVerifications(DateOnly date);
}