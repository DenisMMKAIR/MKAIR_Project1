using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.FGIS.Database.Maintenance;

public class DatabaseMigrator : IDesignTimeDbContextFactory<FGISDatabase>
{
    public FGISDatabase CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<ProjApp.Settings.EmptySettings>(optional: false)
            .Build();
        var optionsBuilder = new DbContextOptionsBuilder<FGISDatabase>();
        optionsBuilder.UseNpgsql(config.GetConnectionString("FGISCache"),
                                 b => b.MigrationsAssembly(typeof(DatabaseMigrator).Assembly));
        optionsBuilder.UseSnakeCaseNamingConvention();
        return new FGISDatabase(optionsBuilder.Options);
    }
}
