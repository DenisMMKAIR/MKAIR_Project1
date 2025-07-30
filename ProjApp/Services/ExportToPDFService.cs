using ProjApp.Database.EntitiesStatic;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class ExportToPDFService
{
    public Task<ServiceResult> ExportToPdfAsync(VerificationGroup group, IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> ExportAllToPdfAsync(VerificationGroup group, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> ExportToPdfAsync(VerificationGroup group, string fileName, Stream stream, string sheetName, string dataRange, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}