using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ProjApp.Database.Maintenance;

public class DatabaseMigrator : IDesignTimeDbContextFactory<ProjDatabase>
{
    public ProjDatabase CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<ProjApp.Settings.EmptySettings>(optional: false)
            .Build();
        var optionsBuilder = new DbContextOptionsBuilder<ProjDatabase>();
        optionsBuilder.UseNpgsql(config.GetConnectionString("default"),
                                 b => b.MigrationsAssembly(typeof(DatabaseMigrator).Assembly));
        optionsBuilder.UseSnakeCaseNamingConvention();
        return new ProjDatabase(optionsBuilder.Options);
    }
}
