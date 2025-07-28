using ProjApp.Database.Entities;

namespace ProjApp.InfrastructureInterfaces;

public interface IGetVerificationsFromExcelProcessor
{
    public IReadOnlyList<IInitialVerification> GetVerificationsFromFile(Stream fileStream, string fileName, string sheetName, string dataRange);
}