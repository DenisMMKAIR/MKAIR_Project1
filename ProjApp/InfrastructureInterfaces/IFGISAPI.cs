namespace ProjApp.InfrastructureInterfaces;

public interface IFGISAPI
{
    Task<IReadOnlyList<FGISDeviceType>> GetDeviceTypesAsync(IReadOnlyList<string> deviceNumbers);

    public record FGISDeviceType(string Number, string Name, string Notation);
}