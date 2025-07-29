using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.ProtocolForms;

public class PDFFilePathManager
{
    private readonly ILogger<PDFFilePathManager> _logger;
    private readonly string _protocolsDirPath;

    public PDFFilePathManager(ILogger<PDFFilePathManager> logger, IConfiguration configuration)
    {
        _logger = logger;

        _protocolsDirPath = configuration["ProtocolsExportDirPath"] ??
            throw new ArgumentNullException(nameof(configuration), "Директория протоколов не задана в настройках");
    }

    public string GetFilePath(IProtocolFileInfo form, string? extraDir = null)
    {
        var dirPath = Path.Combine(
            _protocolsDirPath,
            form.Location.ToString(),
            form.VerificationGroup.ToString(),
            form.VerificationDate.ToString("yyyy-MM"),
            extraDir ?? string.Empty);

        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        var mpi = form.VerifiedUntilDate.Year - form.VerificationDate.Year;
        var fileName = $"{form.VerificationDate:yyyy-MM-dd} № {form.DeviceSerial} (МПИ-{mpi}).pdf";
        fileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c, '_'));

        var filePath = Path.Combine(dirPath, fileName);

        if (File.Exists(filePath))
        {
            _logger.LogWarning("Файл {fileName} уже существует", fileName);
            var i = 2;

            while (File.Exists(filePath) && i < 100)
            {
                fileName = $"{Path.GetFileNameWithoutExtension(fileName)} ({i++}).pdf";
                filePath = Path.Combine(dirPath, fileName);
            }
        }

        return filePath;
    }
}
