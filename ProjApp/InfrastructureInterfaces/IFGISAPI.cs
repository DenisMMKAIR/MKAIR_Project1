using ProjApp.Database.Entities;
using ProjApp.Database.SupportTypes;

namespace ProjApp.InfrastructureInterfaces;

public interface IFGISAPI
{
    Task<IReadOnlyList<DeviceType>> GetDeviceTypesAsync(IReadOnlyList<string> deviceNumbers);
    Task<FGISAPIResult> GetInitialVerificationsAsync(YearMonth date);
}

public class FGISAPIResult
{
    public IReadOnlyList<SuccessInitialVerification>? SuccessInitialVerifications { get; init; }
    public IReadOnlyList<FailedInitialVerification>? FailedInitialVerifications { get; init; }
    public string? Error { get; init; }

    private FGISAPIResult() { }

    public static FGISAPIResult Success(
        IReadOnlyList<SuccessInitialVerification> successInitialVerifications,
        IReadOnlyList<FailedInitialVerification> failedInitialVerifications)
    {
        return new() { SuccessInitialVerifications = successInitialVerifications, FailedInitialVerifications = failedInitialVerifications };
    }

    public static FGISAPIResult Fail(string error) => new() { Error = error };
}