using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Usage;
using WebAPI.Controllers;

namespace WebAPI.Tests;

[TestFixture]
public abstract class ControllersFixture
{
    public ServiceProvider ServiceProvider { get; set; }

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
        services.AddTransient<PendingManometrVerificationsController>();
        services.AddTransient<DeviceTypeController>();
        services.AddTransient<InitialVerificationJobController>();
        services.RegisterProjectDI(configuration);

        ServiceProvider = services.BuildServiceProvider();

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
