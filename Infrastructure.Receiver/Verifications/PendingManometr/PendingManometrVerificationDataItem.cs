using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace Infrastructure.Receiver.Verifications.PendingManometr;

public class PendingManometrVerificationDataItem : IDataItem<PendingManometrVerification>
{
    public string? DeviceTypeNumber { get; set; }
    public string? DeviceSerial { get; set; }
    public DateOnly? Date { get; set; }
    public string? VerificationMethods { get; set; }
    public string? EtalonsNumbers { get; set; }
    public string? OwnerName { get; set; }
    public string? WorkerName { get; set; }
    public double? Temperature { get; set; }
    public string? Pressure { get; set; }
    public double? Hummidity { get; set; }
    public double? Accuracy { get; set; }

    public PendingManometrVerification PostProcess(string fileName, int rowNumber, DeviceLocation location)
    {
        return new()
        {
            DeviceTypeNumber = DeviceTypeNumber!,
            DeviceSerial = DeviceSerial!,
            Date = Date!.Value,
            VerificationMethods = VerificationMethods!.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            EtalonsNumbers = EtalonsNumbers!.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            OwnerName = OwnerName!,
            WorkerName = WorkerName!,
            Temperature = Temperature!.Value,
            Pressure = Pressure!,
            Hummidity = Hummidity!.Value,
            Accuracy = Accuracy,
            Location = location
        };
    }
}