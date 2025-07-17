namespace Infrastructure.FGISAPI.Client;

internal class ClientResponse<T>
{
    public IReadOnlyList<T>? Data { get; init; }
    public bool? AllDataFetched { get; init; }
    public string? Error { get; init; }

    private ClientResponse() { }

    public static ClientResponse<T> Success(IReadOnlyList<T> data, bool allDataFetched) => new() { Data = data, AllDataFetched = allDataFetched };
    public static ClientResponse<T> Fail(string error) => new() { Error = error };
}
