namespace ProjApp.Services.ServiceResults;

public class ServiceItemResult<T>
{
    public T? Item { get; init; }
    public string? Error { get; init; }

    private ServiceItemResult() { }

    public static ServiceItemResult<T> Success(T item) => new() { Item = item };
    public static ServiceItemResult<T> Fail(string error) => new() { Error = error };
}