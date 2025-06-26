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
