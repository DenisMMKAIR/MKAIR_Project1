using Infrastructure.FGIS.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.FGIS.Database;

public class FGISDatabase : DbContext
{
    public FGISDatabase(DbContextOptions<FGISDatabase> options) : base(options) { }

    public DbSet<MonthResult> MonthResults => Set<MonthResult>();
    public DbSet<VerificationId> VerificationIds => Set<VerificationId>();
    public DbSet<Verification> Verifications => Set<Verification>();
    public DbSet<EtalonsId> EtalonIds => Set<EtalonsId>();
    public DbSet<Etalon> Etalons => Set<Etalon>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MonthResult>().HasKey(x => x.Date);
        modelBuilder.Entity<VerificationId>().HasKey(x => x.Vri_id);
        modelBuilder.Entity<Verification>().HasKey(x => x.Vri_id);
        modelBuilder.Entity<Etalon>().HasKey(x => x.Number);
        modelBuilder.Entity<Etalon.EtalonVerificationDocs>().HasKey(x => x.Vri_Id);
        modelBuilder.Entity<EtalonsId>().HasKey(x => x.Rmieta_id);
    }
}
