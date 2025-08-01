using Infrastructure.DocumentProcessor.Creator;
using Microsoft.Extensions.Logging;
using ProjApp.InfrastructureInterfaces;
using ProjApp.ProtocolForms;
using IAppConfig = Microsoft.Extensions.Configuration.IConfiguration;

namespace Infrastructure.DocumentProcessor.Services;

public class TemplateProcessor : ITemplateProcessor
{
    private readonly Dictionary<string, string> _signsCache = [];
    private readonly PDFExporter _exporter;
    private readonly ManometrSuccessDocumentCreator _manDocCreator;
    private readonly DavlenieSuccessDocumentCreator _davDocCreator;
    private bool _disposed;

    public TemplateProcessor(ILogger<TemplateProcessor> logger, IAppConfig configuration)
    {
        var signsDirPath = configuration["SignsDirPath"] ??
            throw new ArgumentNullException(nameof(configuration), "Директория подписей не задана в настройках");

        _exporter = new(logger);
        _manDocCreator = new(_signsCache, signsDirPath);
        _davDocCreator = new(_signsCache, signsDirPath);
    }

    public async Task<PDFCreationResult> CreatePDFAsync(IProtocolForm form, string filePath, CancellationToken cancellationToken = default)
    {
        return form switch
        {
            ManometrForm m => await CreateAsync(_manDocCreator, m, filePath, cancellationToken),
            DavlenieForm d => await CreateAsync(_davDocCreator, d, filePath, cancellationToken),
            _ => PDFCreationResult.Failure("Форма не поддерживается")
        };
    }

    private async Task<PDFCreationResult> CreateAsync<T>(DocumentCreatorBase<T> htmlCreator, T data, string filePath, CancellationToken cancellationToken = default)
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
