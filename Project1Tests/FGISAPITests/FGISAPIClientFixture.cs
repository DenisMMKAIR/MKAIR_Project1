using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            .AddJsonFile("appsettings.Development.json", optional: false)
            .Build();

        var services = new ServiceCollection();
        services.AddLogging(cfg =>
        {
            cfg.AddConfiguration(configuration.GetSection("Logging"));
            cfg.AddConsole();
        });
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
