namespace Infrastructure.DocumentProcessor.Creator;

internal class HTMLCreationResult
{
    public string? HTMLContent { get; init; }
    public string? Error { get; init; }

    private HTMLCreationResult() { }

    public static HTMLCreationResult Success(string htmlContent) => new() { HTMLContent = htmlContent };
    public static HTMLCreationResult Failure(string error) => new() { Error = error };
}
