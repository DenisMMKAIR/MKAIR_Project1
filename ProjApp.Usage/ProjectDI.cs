using Infrastructure.FGISAPI;
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

namespace ProjApp.Usage;

public static class ProjectDI
{
    public static void RegisterProjectDI(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddLogging(cfg => cfg.ClearProviders().AddConsole());

        serviceCollection.AddDbContext<ProjDatabase>(builder =>
            builder.UseNpgsql(configuration.GetConnectionString("default"))
                   .UseSnakeCaseNamingConvention());

        serviceCollection.AddTransient<AddPendingManometrCommand>();
        serviceCollection.AddTransient<AddDeviceTypeCommand>();
        serviceCollection.AddTransient<AddInitialVerificationJobCommand>();

        serviceCollection.AddScoped<IPendingManometrVerificationsProcessor, PendingManometrVerificationExcelProcessor>();
        serviceCollection.AddScoped<PendingManometrVerificationsService>();
        serviceCollection.AddScoped<DeviceTypeService>();
        serviceCollection.AddScoped<InitialVerificationJobService>();

        serviceCollection.AddSingleton<EventKeeper>();
        serviceCollection.AddHostedService<DeviceTypeBackgroundService>();
        serviceCollection.AddHostedService<InitialVerificationBackgroundService>();

        serviceCollection.AddFGISAPI();
    }
}
