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

public class DeviceType1 : DatabaseEntity
{
    public required string Number { get; set; }
    public required string Name { get; set; }
    public required string Notation { get; set; }
}
