using Infrastructure.FGISAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjApp.InfrastructureInterfaces;

namespace Project1Tests.FGISAPITests;

[TestFixture]
public abstract class FGISAPIFixture
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

        services.AddFGISAPI();

        ServiceProvider = services.BuildServiceProvider();

        Client = ServiceProvider.GetRequiredService<IFGISAPI>();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await ServiceProvider.DisposeAsync();
    }
}
