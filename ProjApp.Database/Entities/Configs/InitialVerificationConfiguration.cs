using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjApp.Database.Entities.Configs;

internal class InitialVerificationConfiguration : IEntityTypeConfiguration<InitialVerification>
{
    public void Configure(EntityTypeBuilder<InitialVerification> builder)
    {
        builder.HasIndex(e => new { e.VerificationDate, e.DeviceSerial, e.DeviceTypeNumber })
               .IsUnique();
    }
}
