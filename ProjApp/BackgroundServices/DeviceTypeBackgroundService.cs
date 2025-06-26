using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.InfrastructureInterfaces;

namespace ProjApp.BackgroundServices;

public class DeviceTypeBackgroundService : EventSubscriberBase, IHostedService
{
    private readonly ILogger<DeviceTypeBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IFGISAPI _outerDeviceAPI;
    private readonly EventKeeper _keeper;

    public DeviceTypeBackgroundService(ILogger<DeviceTypeBackgroundService> logger,
                                       IServiceScopeFactory serviceScopeFactory,
                                       IFGISAPI outerDeviceAPI,
                                       EventKeeper keeper) : base(logger)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _outerDeviceAPI = outerDeviceAPI;
        _keeper = keeper;
        SubscribeTo(keeper, BackgroundEvents.GetDevicesType);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // TODO: Decide later. Do we need this? Trigger on program start
        // OnEventTriggered();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override async Task ProcessWorkAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();

        var existsNumbers = await db.DeviceTypes.Select(t => t.Number).ToListAsync();

        var deviceTypesToAdd = await db.PendingManometrVerifications
            .Where(m => !existsNumbers.Contains(m.DeviceTypeNumber))
            .Select(m => m.DeviceTypeNumber)
            .Distinct()
            .ToListAsync();

        if (deviceTypesToAdd.Count == 0) return;

        _logger.LogInformation("{Count} device types to get", deviceTypesToAdd.Count);

        var newAPIDevices = await _outerDeviceAPI.GetDeviceTypesAsync(deviceTypesToAdd);

        var addCommand = scope.ServiceProvider.GetRequiredService<AddDeviceTypeCommand>();

        var newDevices = newAPIDevices
            .Select(d => new DeviceType { Title = d.Title, Number = d.Number, Notation = d.Notation })
            .ToArray();

        var result = await addCommand.ExecuteAsync(newDevices);

        if (result.Error != null)
        {
            _logger.LogError("{Error}", result.Error);
            return;
        }

        _logger.LogInformation("{NewCount} device types added. {DuplicatesCount} duplicates", result.NewCount, result.DuplicatesCount);
        _keeper.Signal(BackgroundEvents.GetDeviceTypeDone);
    }
}
