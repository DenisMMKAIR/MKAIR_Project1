using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjApp.BackgroundServices;
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

        services.RegisterProjectDI(configuration);

        services.AddTransient<DeviceTypeController>();
        services.AddTransient<InitialVerificationJobsController>();
        services.AddTransient<InitialVerificationsController>();
        services.AddTransient<PendingManometrVerificationsController>();
        services.AddTransient<VerificationMethodsController>();

        services.AddSingleton<InitialVerificationBackgroundService>();

        ServiceProvider = services.BuildServiceProvider();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await ServiceProvider.DisposeAsync();
    }
}
