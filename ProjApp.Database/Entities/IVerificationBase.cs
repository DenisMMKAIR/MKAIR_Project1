namespace ProjApp.Database.Entities;

public interface IVerificationBase
{
    Guid Id { get; set; }
    string DeviceTypeNumber { get; set; }
    string DeviceSerial { get; set; }
    DateOnly VerificationDate { get; set; }

    // Navigation properties
    Device? Device { get; set; }
    VerificationMethod? VerificationMethod { get; set; }
    ICollection<Etalon>? Etalons { get; set; }
}