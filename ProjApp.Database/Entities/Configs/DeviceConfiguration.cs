using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjApp.Database.Entities.Configs;

internal class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasIndex(e => new { e.DeviceTypeNumber, e.Serial })
               .IsUnique();
    }
}
