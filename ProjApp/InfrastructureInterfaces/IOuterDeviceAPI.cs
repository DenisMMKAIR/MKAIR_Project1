namespace ProjApp.InfrastructureInterfaces;

public interface IOuterDeviceAPI
{
    Task<DeviceTypeResult?> GetDeviceTypesAsync(IReadOnlyList<string> deviceNumbers);

    public record DeviceTypeResult(string Number, string Name, string Notation);
}