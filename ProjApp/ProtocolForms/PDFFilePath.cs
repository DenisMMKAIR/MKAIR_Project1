using Microsoft.Extensions.Configuration;
using ProjApp.Database.Entities;

namespace ProjApp.ProtocolForms;

public class PDFFilePathManager
{
    private readonly string _protocolsDirPath;

    public PDFFilePathManager(IConfiguration configuration)
    {
        _protocolsDirPath = configuration["ProtocolsExportDirPath"] ??
            throw new ArgumentNullException(nameof(configuration), "Директория протоколов не задана в настройках");
    }

    public string GetFilePath(IProtocolFileInfo form)
    {
        var dirPath = Path.Combine(
            _protocolsDirPath,
            form.Location.ToString(),
            form.VerificationGroup.ToString(),
            form.VerificationDate.ToString("yyyy-MM"));

        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        var mpi = form.VerifiedUntilDate.Year - form.VerificationDate.Year;
        var fileName = $"{form.VerificationDate:yyyy-MM-dd} № {form.DeviceSerial} (МПИ-{mpi}).pdf";
        fileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c, '_'));
        return Path.Combine(dirPath, fileName);
    }
}
