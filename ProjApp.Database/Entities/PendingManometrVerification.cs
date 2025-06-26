using System.Diagnostics.CodeAnalysis;

namespace ProjApp.Database.Entities;

public class PendingManometrVerification : DatabaseEntity
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required DateOnly Date { get; set; }
    public required IReadOnlyList<string> VerificationMethods { get; set; }
    public required IReadOnlyList<string> EtalonsNumbers { get; set; }
    public required string OwnerName { get; set; }
    public required string WorkerName { get; set; }
    public required double Temperature { get; set; }
    public required string Pressure { get; set; }
    public required double Hummidity { get; set; }
    public double? Accuracy { get; set; }
    public required DeviceLocation Location { get; set; }
}
