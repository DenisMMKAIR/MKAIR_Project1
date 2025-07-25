﻿using System.Reflection;
using Infrastructure.DocumentProcessor.Services;
using Infrastructure.FGISAPI;
using Infrastructure.Receiver.Services;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.InfrastructureInterfaces;
using ProjApp.ProtocolForms;
using ProjApp.Services;

namespace ProjApp.Usage;

public static class ProjectDI
{
    public static void RegisterProjectDI(this IServiceCollection serviceCollection, string connectionString, IReadOnlyList<string> assemblies)
    {
        serviceCollection.AddDbContext<ProjDatabase>(builder =>
            builder.UseNpgsql(connectionString)
                   .UseSnakeCaseNamingConvention());

        serviceCollection.AddTransient<AddPendingManometrCommand>();
        serviceCollection.AddTransient<AddEtalonCommand>();
        serviceCollection.AddTransient<AddDeviceCommand>();
        serviceCollection.AddTransient<AddDeviceTypeCommand>();
        serviceCollection.AddTransient<AddInitialVerificationJobCommand>();
        serviceCollection.AddTransient<AddInitialVerificationCommand<SuccessInitialVerification>>();
        serviceCollection.AddTransient<AddInitialVerificationCommand<FailedInitialVerification>>();
        serviceCollection.AddTransient<AddVerificationMethodCommand>();
        serviceCollection.AddTransient<AddOwnerCommand>();
        serviceCollection.AddTransient<AddProtocolTemplateCommand>();

        serviceCollection.AddScoped<IPendingManometrVerificationsProcessor, PendingManometrVerificationExcelProcessor>();
        serviceCollection.AddScoped<IIVSetValuesProcessor, InitialVerificationSetValuesProcessor>();

        serviceCollection.AddScoped<ITemplateProcessor, TemplateProcessor>();

        serviceCollection.AddScoped<DeviceTypeService>();
        serviceCollection.AddScoped<InitialVerificationJobsService>();
        serviceCollection.AddScoped<VerificationsService>();
        serviceCollection.AddScoped<VerificationMethodsService>();
        serviceCollection.AddScoped<OwnersService>();
        serviceCollection.AddScoped<ProtocolTemplesService>();
        serviceCollection.AddScoped<ManometrService>();
        serviceCollection.AddScoped<DavlenieService>();

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan([.. assemblies.Select(Assembly.Load)]);
        serviceCollection.AddSingleton(config);
        serviceCollection.AddScoped<IMapper, ServiceMapper>();

        serviceCollection.AddSingleton<PDFFilePathManager>();
        serviceCollection.AddSingleton<EventKeeper>();
        serviceCollection.AddHostedService<InitialVerificationBackgroundService>();
        serviceCollection.AddHostedService<OwnersBackgroundService>();
        serviceCollection.AddHostedService<CompleteVerificationBackgroundService>();

        serviceCollection.AddFGISAPI();
    }

    public static string? RegisterProjectBaseTestsDI(this IServiceCollection serviceCollection)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json", optional: false)
            .AddUserSecrets<ProjApp.Settings.EmptySettings>(optional: false)
            .Build();

        serviceCollection.AddLogging(cfg =>
        {
            cfg.AddConfiguration(configuration.GetSection("Logging"));
            cfg.AddConsole();
        });

        return configuration.GetConnectionString("test");
    }

    public static void RegisterProjectTestsDI(this IServiceCollection serviceCollection, IReadOnlyList<string> assemblies)
    {
        var connectionString = serviceCollection.RegisterProjectBaseTestsDI();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Connection string is empty");
        }

        serviceCollection.RegisterProjectDI(connectionString, assemblies);
    }
}
