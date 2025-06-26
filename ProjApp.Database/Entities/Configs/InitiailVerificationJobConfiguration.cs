using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjApp.Database.Entities.Configs;

internal class InitialVerificationJobConfiguration : IEntityTypeConfiguration<InitialVerificationJob>
{
    public void Configure(EntityTypeBuilder<InitialVerificationJob> builder)
    {
        builder.HasIndex(e => e.Date)
               .IsUnique();
    }
}
