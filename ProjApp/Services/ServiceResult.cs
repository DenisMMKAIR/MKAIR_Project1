namespace ProjApp.Services;

public class ServiceResult
{
    public string? Message { get; init; }
    public string? Error { get; init; }

    private ServiceResult() { }

    public static ServiceResult Success(string message) => new() { Message = message };
    public static ServiceResult Fail(string error) => new() { Error = error };
}