using Infrastructure.FGISAPI;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Usage;

namespace Project1Tests.FGISAPITests;

[TestFixture]
public abstract class FGISAPIFixture
{
    public ServiceProvider ServiceProvider { get; set; }
    public IFGISAPI Client { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var services = new ServiceCollection();
        services.RegisterProjectBaseTestsDI();
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
