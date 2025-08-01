using Infrastructure.DocumentProcessor.Creator;
using ProjApp.InfrastructureInterfaces;
using ProjApp.ProtocolForms;
using IAppConfig = Microsoft.Extensions.Configuration.IConfiguration;

namespace Infrastructure.DocumentProcessor.Services;

public class TemplateProcessor : ITemplateProcessor
{
    private readonly Browser _browser;
    private readonly ManometrSuccessDocumentCreator _manDocCreator;
    private readonly DavlenieSuccessDocumentCreator _davDocCreator;
    private bool _disposed;

    public TemplateProcessor(IAppConfig configuration)
    {
        _browser = new();

        var signsDirPath = configuration["SignsDirPath"] ??
            throw new ArgumentNullException(nameof(configuration), "Директория подписей не задана в настройках");

        Dictionary<string, string> signsCache = [];

        _manDocCreator = new(_browser, signsCache, signsDirPath);
        _davDocCreator = new(_browser, signsCache, signsDirPath);
    }

    public async Task<PDFCreationResult> CreatePDFAsync(IProtocolForm form, string filePath)
    {
        return form switch
        {
            ManometrForm m => await CreateAsync(_manDocCreator, m, filePath),
            DavlenieForm d => await CreateAsync(_davDocCreator, d, filePath),
            _ => PDFCreationResult.Failure("Форма не поддерживается")
        };
    }

    private static async Task<PDFCreationResult> CreateAsync<T>(DocumentCreatorBase<T> htmlCreator, T data, string filePath)
    {
        var htmlResult = await htmlCreator.CreateAsync(data);

        if (htmlResult.Error != null) return PDFCreationResult.Failure(htmlResult.Error);

        try
        {
            await htmlResult.HTMLPage!.PdfAsync(filePath);
        }
        catch (Exception e)
        {
            return PDFCreationResult.Failure(e.Message);
        }

        return PDFCreationResult.Success(filePath, $"Файл протокола {Path.GetFileName(filePath)} создан");
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        await _browser.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
