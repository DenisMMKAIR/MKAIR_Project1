using Infrastructure.FGIS.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Usage;

namespace Project1Tests.FGISAPITests;

[TestFixture]
public abstract class FGISAPIClientFixture
{
    public ServiceProvider ServiceProvider { get; set; }
    public IFGISAPI Client { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .AddUserSecrets<FGISDatabase>()
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.RegisterProjectDI(configuration);

        ServiceProvider = services.BuildServiceProvider();

        Client = ServiceProvider.GetRequiredService<IFGISAPI>();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await ServiceProvider.DisposeAsync();
    }
}
