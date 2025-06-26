using ProjApp.Database.Entities;

namespace ProjApp.InfrastructureInterfaces;

public interface IFGISAPI
{
    Task<IReadOnlyList<DeviceType>> GetDeviceTypesAsync(IReadOnlyList<string> deviceNumbers);
    Task<GetInitialVerificationType> GetInitialVerifications(string date);

    public record GetInitialVerificationType(
        IReadOnlyList<Etalon> Etalons,
        IReadOnlyList<DeviceType> DeviceTypes,
        IReadOnlyList<InitialVerification> InitialVerifications
    );
}