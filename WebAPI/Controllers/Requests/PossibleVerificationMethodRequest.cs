using ProjApp.Database.EntitiesStatic;

namespace WebAPI.Controllers.Requests;

public class PossibleVerificationMethodRequest
{
    public required ShowVMethods ShowVMethods { get; init; } = ShowVMethods.Новые;
    public string? DeviceTypeNumberFilter { get; init; }
    public string? VerificationNameFilter { get; init; }
    public string? DeviceTypeInfoFilter { get; init; }
    public string? YearMonthFilter { get; init; }
}
