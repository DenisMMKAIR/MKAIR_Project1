using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjApp.Database.Entities.Configs;

internal class InitiailVerificationJobConfiguration : IEntityTypeConfiguration<InitiailVerificationJob>
{
    public void Configure(EntityTypeBuilder<InitiailVerificationJob> builder)
    {
        builder.HasIndex(e => e.Date)
               .IsUnique();
    }
}
