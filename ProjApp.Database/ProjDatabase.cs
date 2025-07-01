using Microsoft.EntityFrameworkCore;
using ProjApp.Database.Entities;
using ProjApp.Database.SupportTypes;

namespace ProjApp.Database;

public class ProjDatabase : DbContext
{
    public ProjDatabase(DbContextOptions<ProjDatabase> options) : base(options) { }

    public DbSet<InitialVerificationJob> InitialVerificationJobs => Set<InitialVerificationJob>();
    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Etalon> Etalons => Set<Etalon>();
    public DbSet<InitialVerification> InitialVerifications => Set<InitialVerification>();
    public DbSet<InitialVerificationFailed> FailedInitialVerifications => Set<InitialVerificationFailed>();
    public DbSet<Protocol> Protocols => Set<Protocol>();
    public DbSet<VerificationMethod> VerificationMethods => Set<VerificationMethod>();

    public DbSet<PendingManometrVerification> PendingManometrVerifications => Set<PendingManometrVerification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InitialVerificationJob>()
            .Property(e => e.Date)
            .HasConversion(new YearMonthConverter());

        modelBuilder.Entity<Protocol>()
            .OwnsMany(p => p.Checkups);
    }
}
