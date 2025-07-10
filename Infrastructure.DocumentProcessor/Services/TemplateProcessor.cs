using Infrastructure.DocumentProcessor.Creator;
using ProjApp.Database.Entities;
using ProjApp.InfrastructureInterfaces;
using IAppConfig = Microsoft.Extensions.Configuration.IConfiguration;

namespace Infrastructure.DocumentProcessor.Services;

public class TemplateProcessor : ITemplateProcessor, IAsyncDisposable
{
    private readonly Dictionary<string, string> _signsCache = [];
    private readonly PDFExporter _exporter = new();
    private readonly ManometrSuccessDocumentCreator _manDocCreator;
    private readonly string _protocoolsDirPath;
    private bool _disposed;

    public TemplateProcessor(IAppConfig configuration)
    {
        var signsDirPath = configuration["SignsDirPath"] ??
            throw new ArgumentNullException(nameof(configuration), "Директория подписей не задана в настройках");

        _protocoolsDirPath = configuration["ProtocolsDirPath"] ??
            throw new ArgumentNullException(nameof(configuration), "Директория протоколов не задана в настройках");

        _manDocCreator = new(_signsCache, signsDirPath);
    }

    public async Task<PDFCreationResult> CreatePDFAsync(ProtocolTemplate template, Manometr1Verification verification)
    {
        var result = await _manDocCreator.CreateAsync(verification);

        if (result.Error != null) return PDFCreationResult.Failure(result.Error);

        var dirPath = Path.Combine(
            _protocoolsDirPath,
            verification.Location.ToString(),
            template.VerificationGroup.ToString(),
            verification.VerificationDate.ToString("yyyy-MM"));

        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        var mpi = verification.VerifiedUntilDate.Year - verification.VerificationDate.Year;
        var fileName = $"{verification.VerificationDate:yyyy-MM-dd} № {verification.DeviceSerial} (МПИ-{mpi}).pdf";
        var filePath = Path.Combine(dirPath, fileName);

        await _exporter.ExportAsync(result.HTMLContent!, filePath);

        return PDFCreationResult.Success(filePath, $"Файл протокола {fileName} создан");
    }

    public Task<PDFCreationResult> CreatePDFAsync(ProtocolTemplate template, FailedCompleteVerification verification)
    {
        throw new NotImplementedException();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        await _exporter.DisposeAsync();
    }
}
