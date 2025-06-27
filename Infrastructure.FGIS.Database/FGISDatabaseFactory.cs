using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.FGIS.Database;

public class FGISDatabaseFactory
{
    public static FGISDatabase Create()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<FGISDatabase>()
            .Build();
        var options = new DbContextOptionsBuilder<FGISDatabase>()
            .UseNpgsql(configuration.GetConnectionString("FGISCache"))
            .UseSnakeCaseNamingConvention()
            .Options;
        return new FGISDatabase(options);
    }
}
