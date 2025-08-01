namespace ProjApp.Database.Entities;

public interface ICompleteVerification : IVerificationBase, IProtocolFileInfo
{
    // public Guid Id { get; set; } // Inherited
    public string ProtocolNumber { get; set; }
    // public string DeviceTypeNumber { get; set; } // Inherited
    // public string DeviceSerial { get; set; } // Inherited
    // public DateOnly VerificationDate { get; set; } // Inherited
    // public VerificationGroup VerificationGroup { get; set; } // Inherited
    // public DeviceLocation Location { get; set; } // Inherited

    // Navigation properties
    // public Device? Device { get; set; } // Inherited
    // public VerificationMethod? VerificationMethod { get; set; } // Inherited
    public Owner? Owner { get; set; }
    // public ICollection<Etalon>? Etalons { get; set; } // Inherited
}
