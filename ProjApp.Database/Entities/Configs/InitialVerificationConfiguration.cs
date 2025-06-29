using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjApp.Database.Entities.Configs;

internal class InitialVerificationConfiguration<T> : IEntityTypeConfiguration<T> where T : class, IInitialVerification
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasIndex(e => new { e.VerificationDate, e.DeviceSerial, e.DeviceTypeNumber })
               .IsUnique();
    }
}
