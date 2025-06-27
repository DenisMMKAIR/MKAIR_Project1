using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.FGIS.Database;

public class FGISDatabaseFactory
{
    public static void Configure(DbContextOptionsBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<ProjApp.Settings.EmptySettings>(optional: false)
            .Build();
        builder.UseNpgsql(configuration.GetConnectionString("FGISCache"))
               .UseSnakeCaseNamingConvention();
    }
}
