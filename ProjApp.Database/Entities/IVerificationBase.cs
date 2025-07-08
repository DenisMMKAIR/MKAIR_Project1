namespace ProjApp.Database.Entities;

public interface IVerificationBase
{
    string DeviceTypeNumber { get; set; }
    string DeviceSerial { get; set; }
    DateOnly VerificationDate { get; set; }
}