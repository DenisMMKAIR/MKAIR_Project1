using Microsoft.Extensions.DependencyInjection;
using WebAPI.Controllers;

namespace WebAPI.Tests;

public class DeviceTypeControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        var controller = ServiceProvider.GetRequiredService<DeviceTypeController>();
        var response = await controller.AddDeviceType(new() { Name = "test", Number = "test", Notation = "test" });
        Assert.That(response.Message, Is.EqualTo("Тип устройства test успешно добавлен"));
    }

    [Test]
    public async Task Test2()
    {
        var controller = ServiceProvider.GetRequiredService<DeviceTypeController>();
        var response = await controller.GetDevicesPaginated(new());
        Assert.That(response.Data, Is.Not.Null);
        Assert.That(response.Data.Items, Has.Count.EqualTo(1));
    }
}
