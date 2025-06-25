using Infrastructure.Receiver.Services;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using Microsoft.Extensions.Configuration;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Services;
using Microsoft.EntityFrameworkCore;
using ProjApp.Database.Commands;

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

        serviceCollection.AddScoped<IPendingManometrVerificationsProcessor, PendingManometrVerificationExcelProcessor>();
        serviceCollection.AddScoped<PendingManometrVerificationsService>();
        serviceCollection.AddScoped<DeviceTypeService>();

        serviceCollection.AddSingleton<IOuterDeviceAPI, DUMMYOuterDeviceAPI>();
        serviceCollection.AddSingleton<EventKeeper>();
        serviceCollection.AddHostedService<DeviceTypeBackgroundService>();
    }
}

internal class DUMMYOuterDeviceAPI : IOuterDeviceAPI
{
    public Task<IOuterDeviceAPI.DeviceTypeResult?> GetDeviceTypesAsync(IReadOnlyList<string> deviceNumbers)
    {
        return Task.FromResult<IOuterDeviceAPI.DeviceTypeResult?>(null);
    }
}