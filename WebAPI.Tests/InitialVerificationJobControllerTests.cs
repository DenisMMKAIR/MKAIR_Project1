using Microsoft.Extensions.DependencyInjection;
using WebAPI.Controllers;

namespace WebAPI.Tests;

public class InitialVerificationJobControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        var controller = ServiceProvider.GetRequiredService<InitialVerificationJobController>();
        var response = await controller.AddJob(new(2024, 02));
        Assert.Multiple(() =>
        {
            Assert.That(response.Message, Is.EqualTo("Задание добавлено"));
            Assert.That(response.Error, Is.Null);
        });
    }

    [Test]
    public async Task Test2()
    {
        var controller = ServiceProvider.GetRequiredService<InitialVerificationJobController>();
        var result = await controller.AddJob(new(2023, 02));
        Assert.That(result.Error, Is.EqualTo("Год от 2024 до текущего"));
    }

    [Test]
    public async Task Test3()
    {
        var controller = ServiceProvider.GetRequiredService<InitialVerificationJobController>();
        var response = await controller.GetJobs(new());
        Assert.Multiple(() =>
        {
            Assert.That(response.Data, Is.Not.Null);
            Assert.That(response.Error, Is.Null);
        });
        Assert.That(response.Data.Items, Has.Count.EqualTo(1));
    }
}
