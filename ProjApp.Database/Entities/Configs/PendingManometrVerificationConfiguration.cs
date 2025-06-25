using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjApp.Database.Entities.Configs;

internal class PendingManometrVerificationConfiguration : IEntityTypeConfiguration<PendingManometrVerification>
{
    public void Configure(EntityTypeBuilder<PendingManometrVerification> builder)
    {
        builder.HasIndex(e => new { e.DeviceTypeNumber, e.DeviceSerial, e.Date, e.Location })
               .IsUnique();
    }
}

public class PendingManometrVerification1 : DatabaseEntity
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
