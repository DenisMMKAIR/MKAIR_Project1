using Infrastructure.FGIS.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.FGIS.Database;

public class FGISDatabase : DbContext
{
    public FGISDatabase(DbContextOptions<FGISDatabase> options) : base(options) { }

    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Etalon> Etalons => Set<Etalon>();
    public DbSet<Verification> Verifications => Set<Verification>();
    public DbSet<MonthResult> MonthResults => Set<MonthResult>();
}
