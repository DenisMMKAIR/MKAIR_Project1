namespace WebAPI.Controllers.Requests;

public class PossibleVerificationMethodRequest
{
    public string? DeviceTypeNumberFilter { get; init; }
    public string? VerificationNameFilter { get; init; }
    public string? DeviceTypeInfoFilter { get; init; }
    public string? YearMonthFilter { get; init; }
    public bool? ShowAllTypeNumbers { get; init; }
}
