using Infrastructure.FGIS.Database;
using Infrastructure.FGISAPI.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Usage;

namespace Project1Tests.FGISAPITests;

[TestFixture]
public abstract class FGISAPIClientFixture
{
    public ServiceProvider ServiceProvider { get; set; }
    public FGISAPIClient Client { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .AddUserSecrets<FGISDatabase>()
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(cfg => cfg.AddConsole());
        services.RegisterProjectDI(configuration);

        ServiceProvider = services.BuildServiceProvider();

        Client = ServiceProvider.GetRequiredService<FGISAPIClient>();

        var db = ServiceProvider.GetRequiredService<ProjDatabase>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await ServiceProvider.DisposeAsync();
    }
}
