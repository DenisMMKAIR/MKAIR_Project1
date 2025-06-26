using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjApp.Database.Entities.Configs;

internal class DeviceTypeConfiguration : IEntityTypeConfiguration<DeviceType>
{
    public void Configure(EntityTypeBuilder<DeviceType> builder)
    {
        builder.HasIndex(e => e.Number)
               .IsUnique();
    }
}
