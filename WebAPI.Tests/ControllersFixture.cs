using Microsoft.Extensions.DependencyInjection;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Usage;
using WebAPI.Controllers;

namespace WebAPI.Tests;

[TestFixture]
public abstract class ControllersFixture
{
    private ServiceProvider _serviceProvider;
    public IServiceScopeFactory ScopeFactory { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var services = new ServiceCollection();
        services.RegisterProjectTestsDI();

        services.AddTransient<DeviceTypeController>();
        services.AddTransient<InitialVerificationJobsController>();
        services.AddTransient<InitialVerificationsController>();
        services.AddTransient<PendingManometrVerificationsController>();
        services.AddTransient<VerificationMethodsController>();
        services.AddTransient<ProtocolTemplateController>();

        services.AddSingleton<InitialVerificationBackgroundService>();

        _serviceProvider = services.BuildServiceProvider();
        ScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

        using var scope = ScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _serviceProvider.DisposeAsync();
    }
}
