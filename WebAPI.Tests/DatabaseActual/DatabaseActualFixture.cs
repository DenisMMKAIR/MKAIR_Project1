using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjApp.BackgroundServices;
using ProjApp.Usage;
using WebAPI.Controllers;

namespace WebAPI.Tests.DatabaseActual;

[TestFixture]
public class DatabaseActualFixture
{
    private ServiceProvider _serviceProvider;
    public IServiceScopeFactory ScopeFactory { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var services = new ServiceCollection();

        var assemblies = new string?[]
            {
                typeof(ProjApp.Mapping.SuccessInitialVerificationDto).Assembly.FullName,
                typeof(Controllers.Requests.AddDeviceTypeRequest).Assembly.FullName
            }
            .Select(name => name ?? throw new InvalidOperationException("No assembly name"))
            .ToArray();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json", optional: false)
            .AddUserSecrets<ProjApp.Settings.EmptySettings>(optional: false)
            .Build();

        services.AddLogging(cfg =>
        {
            cfg.AddConfiguration(configuration.GetSection("Logging"));
            cfg.AddConsole();
        });

        var connectionString = configuration.GetConnectionString("default") ??
            throw new InvalidOperationException("No connection string");

        services.RegisterProjectDI(connectionString, assemblies);
        services.AddTransient<DeviceTypeController>();
        services.AddTransient<InitialVerificationJobsController>();
        services.AddTransient<VerificationsController>();
        services.AddTransient<VerificationMethodsController>();

        services.AddSingleton<InitialVerificationBackgroundService>();

        _serviceProvider = services.BuildServiceProvider();
        ScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _serviceProvider.DisposeAsync();
    }
}
