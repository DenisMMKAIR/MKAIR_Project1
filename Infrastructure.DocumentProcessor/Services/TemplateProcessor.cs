using Infrastructure.DocumentProcessor.Creator;
using ProjApp.InfrastructureInterfaces;
using ProjApp.ProtocolForms;
using IAppConfig = Microsoft.Extensions.Configuration.IConfiguration;

namespace Infrastructure.DocumentProcessor.Services;

public class TemplateProcessor : ITemplateProcessor
{
    private readonly Dictionary<string, string> _signsCache = [];
    private readonly PDFExporter _exporter = new();
    private readonly ManometrSuccessDocumentCreator _manDocCreator;
    private readonly DavlenieSuccessDocumentCreator _davDocCreator;
    private bool _disposed;

    public TemplateProcessor(IAppConfig configuration)
    {
        var signsDirPath = configuration["SignsDirPath"] ??
            throw new ArgumentNullException(nameof(configuration), "Директория подписей не задана в настройках");

        _manDocCreator = new(_signsCache, signsDirPath);
        _davDocCreator = new(_signsCache, signsDirPath);
    }

    public async Task<PDFCreationResult> CreatePDFAsync(ManometrForm verification, string filePath, CancellationToken? cancellationToken = null)
    {
        return await CreateAsync(_manDocCreator, verification, filePath, cancellationToken);
    }

    public async Task<PDFCreationResult> CreatePDFAsync(DavlenieForm verification, string filePath, CancellationToken? cancellationToken = null)
    {
        return await CreateAsync(_davDocCreator, verification, filePath, cancellationToken);
    }

    private async Task<PDFCreationResult> CreateAsync<T>(DocumentCreatorBase<T> htmlCreator, T data, string filePath, CancellationToken? cancellationToken = null)
    {
        var htmlResult = await htmlCreator.CreateAsync(data, cancellationToken);

        if (htmlResult.Error != null) return PDFCreationResult.Failure(htmlResult.Error);

        await _exporter.ExportAsync(htmlResult.HTMLContent!, filePath, cancellationToken);

        return PDFCreationResult.Success(filePath, $"Файл протокола {Path.GetFileName(filePath)} создан");
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        await _exporter.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
