using ProjApp.ProtocolForms;

namespace ProjApp.InfrastructureInterfaces;

public interface ITemplateProcessor : IAsyncDisposable
{
    Task<PDFCreationResult> CreatePDFAsync(ManometrForm verification, string filePath, CancellationToken? cancellationToken = null);
    Task<PDFCreationResult> CreatePDFAsync(DavlenieForm verification, string filePath, CancellationToken? cancellationToken = null);
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