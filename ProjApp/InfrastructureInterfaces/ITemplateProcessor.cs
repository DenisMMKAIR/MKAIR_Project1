using ProjApp.Database.Entities;

namespace ProjApp.InfrastructureInterfaces;

public interface ITemplateProcessor : IAsyncDisposable
{
    Task<PDFCreationResult> CreatePDFAsync(Manometr1Verification verification);
}

public class PDFCreationResult
{
    public string? FilePath { get; init; }
    public string? Message { get; init; }
    public string? Error { get; init; }

    private PDFCreationResult() { }

    public static PDFCreationResult Success(string FilePath, string message) => new() { FilePath = FilePath, Message = message };
    public static PDFCreationResult Failure(string error) => new() { Error = error };
}