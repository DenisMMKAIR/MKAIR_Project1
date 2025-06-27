using Infrastructure.Receiver.Services;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using Microsoft.Extensions.Configuration;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Services;
using Microsoft.EntityFrameworkCore;
using ProjApp.Database.Commands;
using Infrastructure.FGISAPI;

namespace ProjApp.Usage;

public static class ProjectDI
{
    public static void RegisterProjectDI(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
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
