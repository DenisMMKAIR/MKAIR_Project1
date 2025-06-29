using ProjApp.Database;

namespace ProjApp.Services;

public class ServicePaginatedResult<T>
{
    public string? Message { get; init; }
    public string? Error { get; init; }
    public PaginatedList<T>? Data { get; init; }

    private ServicePaginatedResult() { }

    public static ServicePaginatedResult<T> Success(PaginatedList<T> data, string? message = null)
        => new() { Data = data, Message = message ?? $"Получено {data.Items.Count} записей" };
    public static ServicePaginatedResult<T> Fail(string error) => new() { Error = error };
}
