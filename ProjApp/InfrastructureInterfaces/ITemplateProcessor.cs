using ProjApp.ProtocolForms;

namespace ProjApp.InfrastructureInterfaces;

public interface ITemplateProcessor : IAsyncDisposable
{
    Task<PDFCreationResult> CreatePDFAsync(IProtocolForm protocolForm, string filePath, CancellationToken cancellationToken);
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