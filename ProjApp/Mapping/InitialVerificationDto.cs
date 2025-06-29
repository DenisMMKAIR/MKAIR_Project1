using ProjApp.Database.Entities;

namespace ProjApp.Mapping;

public class InitialVerificationDto
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required string DeviceTypeInfo { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }
    public required string VerificationTypeName { get; set; }
    public required string Owner { get; set; }
    public required IReadOnlyList<string> Etalons { get; set; }
    public required Guid Id { get; set; }

    // Optional
    public string? AdditionalInfo { get; set; }

    public static InitialVerificationDto MapTo(InitialVerification initialVerification) => new()
    {
        DeviceTypeNumber = initialVerification.DeviceTypeNumber,
        DeviceSerial = initialVerification.DeviceSerial,
        VerificationDate = initialVerification.VerificationDate,
        DeviceTypeInfo = $"{initialVerification.Device!.DeviceType!.Title} {initialVerification.Device!.DeviceType!.Notation}",
        VerifiedUntilDate = initialVerification.VerifiedUntilDate,
        VerificationTypeName = initialVerification.VerificationTypeName,
        Owner = initialVerification.Owner,
        AdditionalInfo = initialVerification.AdditionalInfo,
        Etalons = [.. initialVerification.Etalons!.Select(e => e.Number)],
        Id = initialVerification.Id
    };
}
