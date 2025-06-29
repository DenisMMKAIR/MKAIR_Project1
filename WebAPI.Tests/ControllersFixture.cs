using Infrastructure.Receiver.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Services;
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

        services.AddDbContext<ProjDatabase>(builder =>
            builder.UseNpgsql(configuration.GetConnectionString("default"))
                   .UseSnakeCaseNamingConvention());

        services.AddTransient<AddPendingManometrCommand>();
        services.AddTransient<AddEtalonCommand>();
        services.AddTransient<AddDeviceCommand>();
        services.AddTransient<AddDeviceTypeCommand>();
        services.AddTransient<AddInitialVerificationJobCommand>();
        services.AddTransient<AddInitialVerificationCommand>();

        services.AddScoped<IPendingManometrVerificationsProcessor, PendingManometrVerificationExcelProcessor>();
        services.AddScoped<PendingManometrVerificationsService>();
        services.AddScoped<DeviceTypeService>();
        services.AddScoped<InitialVerificationJobService>();

        services.AddSingleton<EventKeeper>();

        services.AddTransient<PendingManometrVerificationsController>();
        services.AddTransient<DeviceTypeController>();
        services.AddTransient<InitialVerificationJobController>();

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
