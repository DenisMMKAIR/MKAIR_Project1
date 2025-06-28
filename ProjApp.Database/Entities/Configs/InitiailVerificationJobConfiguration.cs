using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjApp.Database.SupportTypes;

namespace ProjApp.Database.Entities.Configs;

internal class InitialVerificationJobConfiguration : IEntityTypeConfiguration<InitialVerificationJob>
{
    public void Configure(EntityTypeBuilder<InitialVerificationJob> builder)
    {
        builder.Property(e => e.Date)
            .HasConversion(new YearMonthConverter());

        builder.HasIndex(e => e.Date)
               .IsUnique();
    }
}
