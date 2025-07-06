using ProjApp.Database.Entities;
using ProjApp.Services;

namespace ProjApp.InfrastructureInterfaces;

public interface IIVSetValuesProcessor
{
    public IReadOnlyList<IInitialVerification> SetFromExcelFile(Stream fileStream, SetValuesRequest request);
}