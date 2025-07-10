using Microsoft.Extensions.DependencyInjection;
using WebAPI.Controllers;

namespace WebAPI.Tests.DatabaseTemp;

public class DeviceTypeControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        using var scope = ScopeFactory.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService<DeviceTypeController>();
        var response1 = await controller.AddDeviceType(new() { Name = "test", Number = "test", Notation = "test" });
        var response2 = await controller.AddDeviceType(new() { Name = "test", Number = "test", Notation = "test" });
        Assert.Multiple(() =>
        {
            Assert.That(response1.Message, Is.EqualTo("Тип устройства test успешно добавлен"));
            Assert.That(response2.Error, Is.EqualTo("Тип устройства уже существует"));
        });
    }

    [Test]
    public async Task Test2()
    {
        using var scope = ScopeFactory.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService<DeviceTypeController>();
        var response = await controller.GetDevicesPaginated(new());
        Assert.That(response.Data!.Items, Has.Count.EqualTo(1));
    }
}
