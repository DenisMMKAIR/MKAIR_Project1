namespace Infrastructure.SharedCode;

public interface IDocumentData { }

public class ManometrData : IDocumentData
{
    public required string Address { get; set; }
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required string DeviceTypeModification { get; set; }
    public required uint ManufactureYear { get; set; }
    public required string Owner { get; set; }
    public required ulong OwnerINN { get; set; }
    public required DateOnly VerificationDate { get; set; }
    // public required DateOnly NextVerificationDate { get; set; }
    public required string Worker { get; set; }
    public required float Temperature { get; set; }
    public required string Pressure { get; set; }
    public required byte Humidity { get; set; }
    public required float Accuracy { get; set; }
    public required string ProtocolNumber { get; set; }
    public required double MeasurementMin { get; set; }
    public required double MeasurementMax { get; set; }
    public required string MeasurementUnit { get; set; }
    public required IReadOnlyList<string> Etalons { get; set; }
    public required IReadOnlyList<string> VerificationsName { get; set; }
    public required string DeviceTypeName { get; set; }
    public required string VerificationVisualCheckup { get; set; }
    public required string VerificationResultCheckup { get; set; }
    public required string VerificationAccuracyCheckup { get; set; }
    public required string FileName { get; set; }

    public override string ToString() => $"[{ProtocolNumber}][{DeviceTypeNumber}][{DeviceSerial}]";
}
