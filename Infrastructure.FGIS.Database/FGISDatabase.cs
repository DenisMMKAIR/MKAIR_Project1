using Infrastructure.FGIS.Database.Entities;
using Microsoft.EntityFrameworkCore;
using ProjApp.Database.SupportTypes;

namespace Infrastructure.FGIS.Database;

public class FGISDatabase : DbContext
{
    public FGISDatabase(DbContextOptions<FGISDatabase> options) : base(options) { }

    public DbSet<MonthResult> MonthResults => Set<MonthResult>();
    public DbSet<VerificationId> VerificationIds => Set<VerificationId>();
    public DbSet<VerificationWithEtalon> VerificationsWithEtalon => Set<VerificationWithEtalon>();
    public DbSet<VerificationWithSes> VerificationsWithtSes => Set<VerificationWithSes>();
    public DbSet<EtalonsId> EtalonIds => Set<EtalonsId>();
    public DbSet<Etalon> Etalons => Set<Etalon>();
    public DbSet<DeviceTypeId> DeviceTypeIds => Set<DeviceTypeId>();
    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MonthResult>().HasKey(x => x.Date);
        modelBuilder.Entity<MonthResult>().Property(x => x.Date).HasConversion(new YearMonthConverter());

        modelBuilder.Entity<VerificationId>().HasKey(x => x.Vri_id);
        modelBuilder.Entity<VerificationId>().Property(x => x.Date).HasConversion(new YearMonthConverter());

        modelBuilder.Entity<VerificationWithEtalon>().HasKey(x => x.Vri_id);
        modelBuilder.Entity<VerificationWithEtalon>(x => x.OwnsOne(e => e.Info));
        modelBuilder.Entity<VerificationWithEtalon>(x => x.OwnsOne(e => e.MiInfo, mi => mi.OwnsOne(m => m.SingleMI)));
        modelBuilder.Entity<VerificationWithEtalon>(x => x.OwnsOne(e => e.VriInfo, vr => vr.OwnsOne(v => v.Applicable)));
        modelBuilder.Entity<VerificationWithEtalon>(x => x.OwnsOne(e => e.VriInfo, vr => vr.OwnsOne(v => v.Inapplicable)));
        // Also they ALWAYS lazy load
        modelBuilder.Entity<VerificationWithEtalon>(x => x.OwnsOne(e => e.Means, m => m.OwnsMany(mm => mm.Mieta)));

        modelBuilder.Entity<VerificationWithSes>().HasKey(x => x.Vri_id);
        modelBuilder.Entity<VerificationWithSes>(x => x.OwnsOne(e => e.Info));
        modelBuilder.Entity<VerificationWithSes>(x => x.OwnsOne(e => e.MiInfo, mi => mi.OwnsOne(m => m.SingleMI)));
        modelBuilder.Entity<VerificationWithSes>(x => x.OwnsOne(e => e.VriInfo, vr => vr.OwnsOne(v => v.Applicable)));
        modelBuilder.Entity<VerificationWithSes>(x => x.OwnsOne(e => e.VriInfo, vr => vr.OwnsOne(v => v.Inapplicable)));
        modelBuilder.Entity<VerificationWithSes>(x => x.OwnsOne(e => e.Means, m => m.OwnsMany(mm => mm.Ses)));

        modelBuilder.Entity<EtalonsId>().HasKey(x => new { x.Rmieta_id, x.Date });
        modelBuilder.Entity<EtalonsId>().Property(x => x.Date).HasConversion(new YearMonthConverter());

        modelBuilder.Entity<Etalon>().HasKey(x => x.Number);
        // Also they ALWAYS lazy load
        modelBuilder.Entity<Etalon>().OwnsMany(e => e.CResults);

        modelBuilder.Entity<DeviceTypeId>().HasKey(x => x.MIT_UUID);
        modelBuilder.Entity<DeviceTypeId>().HasIndex(x => x.Number).IsUnique();

        modelBuilder.Entity<DeviceType>().HasKey(x => x.Id);
        modelBuilder.Entity<DeviceType>().HasIndex(x => x.Number).IsUnique();
        modelBuilder.Entity<DeviceType>().OwnsMany(x => x.Meth);
        modelBuilder.Entity<DeviceType>().OwnsMany(x => x.Spec);
        modelBuilder.Entity<DeviceType>().OwnsMany(x => x.Manufacturer);
    }
}
