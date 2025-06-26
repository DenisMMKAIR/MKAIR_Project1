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
        var response = await controller.AddJob(new(2024, 01));
        Assert.That(response.Error, Is.Not.Null);
        Assert.That(response.Error, Is.EqualTo("Неверно задана дата. От 2024.02 До текущей даты"));
    }

    [Test]
    public async Task Test3()
    {
        var controller = ServiceProvider.GetRequiredService<InitialVerificationJobController>();
        var response = await controller.AddJob(new(0, 0));
        Assert.That(response.Error, Is.Not.Null);
        Assert.That(response.Error, Is.EqualTo("Неверно задан год. От 2024"));
    }

    [Test]
    public async Task Test4()
    {
        var controller = ServiceProvider.GetRequiredService<InitialVerificationJobController>();
        var response = await controller.GetJobs(new());
        Assert.Multiple(() =>
        {
            Assert.That(response.Data, Is.Not.Null);
            Assert.That(response.Error, Is.Null);
        });
        Assert.That(response.Data.Items.Count, Is.EqualTo(1));
    }
}
