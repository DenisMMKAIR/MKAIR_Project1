using Infrastructure.Receiver.Verifications.PendingManometr;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.InfrastructureInterfaces;

namespace Infrastructure.Receiver.Services;

public class PendingManometrVerificationExcelProcessor : IPendingManometrVerificationsProcessor
{
    private readonly ExcelFileProcessor<PendingManometrVerificationDataItem, PendingManometrVerification> _excelProcessor;

    public PendingManometrVerificationExcelProcessor(ILogger<PendingManometrVerificationExcelProcessor> logger)
    {
        _excelProcessor = new(logger);
    }

    public IReadOnlyList<PendingManometrVerification> ReadVerificationFile(Stream fileStream, string fileName, string sheetName, string dataRange, DeviceLocation location)
    {
        return _excelProcessor.ReadVerificationFile(fileStream, fileName, sheetName, dataRange, new PendingManometrVerificationsColumnsSetup(), location);
    }
}
