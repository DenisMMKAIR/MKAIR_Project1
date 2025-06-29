using Infrastructure.FGISAPI;
using Infrastructure.Receiver.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Services;

namespace ProjApp.Usage;

public static class ProjectDI
{
    public static void RegisterProjectDI(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<ProjDatabase>(builder =>
            builder.UseNpgsql(configuration.GetConnectionString("default"))
                   .UseSnakeCaseNamingConvention());

        serviceCollection.AddTransient<AddPendingManometrCommand>();
        serviceCollection.AddTransient<AddEtalonCommand>();
        serviceCollection.AddTransient<AddDeviceCommand>();
        serviceCollection.AddTransient<AddDeviceTypeCommand>();
        serviceCollection.AddTransient<AddInitialVerificationJobCommand>();
        serviceCollection.AddTransient<AddInitialVerificationCommand<InitialVerification>>();
        serviceCollection.AddTransient<AddInitialVerificationCommand<InitialVerificationFailed>>();

        serviceCollection.AddScoped<IPendingManometrVerificationsProcessor, PendingManometrVerificationExcelProcessor>();
        serviceCollection.AddScoped<PendingManometrVerificationsService>();
        serviceCollection.AddScoped<DeviceTypeService>();
        serviceCollection.AddScoped<InitialVerificationJobsService>();
        serviceCollection.AddScoped<InitialVerificationService>();

        serviceCollection.AddSingleton<EventKeeper>();
        serviceCollection.AddHostedService<DeviceTypeBackgroundService>();
        serviceCollection.AddHostedService<InitialVerificationBackgroundService>();

        serviceCollection.AddFGISAPI();
    }
}
