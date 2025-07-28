using ProjApp.Database.Entities;

namespace ProjApp.InfrastructureInterfaces;

public interface IGetVerificationsFromExcelProcessor
{
    public IReadOnlyList<IVerificationBase> GetVerificationsFromFile(Stream fileStream, string fileName, string sheetName, string dataRange);
}