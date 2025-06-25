using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjApp.Database;
using ProjApp.InfrastructureInterfaces;

namespace ProjApp.BackgroundServices;

public class DeviceTypeBackgroundService : EventSubscriberBase, IHostedService
{
    private readonly ILogger<DeviceTypeBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IOuterDeviceAPI _outerDeviceAPI;

    public DeviceTypeBackgroundService(ILogger<DeviceTypeBackgroundService> logger,
                                       IServiceScopeFactory serviceScopeFactory,
                                       IOuterDeviceAPI outerDeviceAPI,
                                       EventKeeper keeper) : base(logger)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _outerDeviceAPI = outerDeviceAPI;
        SubscribeTo(keeper, BackgroundEvents.GetDevicesType);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        OnEventTriggered();
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
            .ToListAsync();
        _logger.LogInformation("{Count} device types to get", deviceTypesToAdd.Count);
        var newDevices = await _outerDeviceAPI.GetDeviceTypesAsync(deviceTypesToAdd);
        
    }
}
