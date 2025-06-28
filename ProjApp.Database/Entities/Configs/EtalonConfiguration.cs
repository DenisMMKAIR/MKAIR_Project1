using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjApp.Database.Entities.Configs;

internal class EtalonConfiguration : IEntityTypeConfiguration<Etalon>
{
    public void Configure(EntityTypeBuilder<Etalon> builder)
    {
        builder.HasIndex(e => new { e.Number, e.Date })
               .IsUnique();
    }
}
