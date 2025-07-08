using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Usage;
using WebAPI.Controllers;

namespace WebAPI.Tests;

public class FreeTests : RealDBTests
{
    [Test]
    public async Task Test1()
    {
        using var scope = ScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
        var ivs = await db.SuccessInitialVerifications
            .Where(iv => iv.VerificationDate.Year == 2025 && iv.VerificationDate.Month == 2)
            .GroupBy(iv => iv.Device!.DeviceType!.Title.Substring(0, iv.Device.DeviceType.Title.IndexOf(" ")))
            .Select(g => new { g.Key, Titles = g.Select(iv => iv.Device!.DeviceType!.Title).Distinct().Order().ToArray() })
            .OrderBy(g => g.Key)
            .ToArrayAsync();
    }
}

[TestFixture]
public class RealDBTests
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
                typeof(WebAPI.Controllers.Requests.AddDeviceTypeRequest).Assembly.FullName
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
        services.AddTransient<PendingManometrVerificationsController>();
        services.AddTransient<VerificationMethodsController>();
        services.AddTransient<ProtocolTemplateController>();

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
