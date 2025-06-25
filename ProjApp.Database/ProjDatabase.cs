using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ProjApp.Database.Entities;

namespace ProjApp.Database;

public class ProjDatabase : DbContext
{
    public ProjDatabase(DbContextOptions<ProjDatabase> options) : base(options) { }

    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();
    public DbSet<PendingManometrVerification> PendingManometrVerifications => Set<PendingManometrVerification>();
    public DbSet<Owner> Owners => Set<Owner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(ProjDatabase))!);
        base.OnModelCreating(modelBuilder);
    }
}
