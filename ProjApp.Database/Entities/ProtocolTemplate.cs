
using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Database.Entities;

public class ProtocolTemplate : DatabaseEntity
{
    public required IReadOnlyList<string> DeviceTypeNumbers { get; set; }
    public required ProtocolGroup Group { get; set; }
    public required bool VerificationSucces { get; set; }
    public required IDictionary<string, string> Checkups { get; set; }
    public required IDictionary<string, object> Values { get; set; }

    // Navigation properties
    public ICollection<VerificationMethod>? VerificationMethods { get; set; }
}

public interface ICompleteVerification : IVerificationBase
{
    // Navigation properties
    ProtocolTemplate? ProtocolTemplate { get; set; }
}

public class SuccessCompleteVerification : DatabaseEntity, ICompleteVerification
{
    public ProtocolTemplate? ProtocolTemplate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string DeviceTypeNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string DeviceSerial { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateOnly VerificationDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Device? Device { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public ICollection<Etalon>? Etalons { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}

public class FauiledCompleteVerification : DatabaseEntity, ICompleteVerification
{
    public ProtocolTemplate? ProtocolTemplate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string DeviceTypeNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string DeviceSerial { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateOnly VerificationDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Device? Device { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public ICollection<Etalon>? Etalons { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
