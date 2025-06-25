namespace Infrastructure.DocumentProcessor.Creator;

public class CreationResult
{
    public string? HTMLContent { get; init; }
    public string? Error { get; init; }

    private CreationResult() { }

    public static CreationResult Success(string htmlContent) => new() { HTMLContent = htmlContent };
    public static CreationResult Failure(string error) => new() { Error = error };
}
