using Microsoft.Extensions.DependencyInjection;
using WebAPI.Controllers;

namespace WebAPI.Tests.DatabaseTemp;

public class InitialVerificationJobControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        using var scope = ScopeFactory.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService<InitialVerificationJobsController>();
        var response1 = await controller.AddJob(new(2024, 02));
        var response2 = await controller.AddJob(new(2024, 02));
        Assert.Multiple(() =>
        {
            Assert.That(response1.Message, Is.EqualTo("Задание добавлено"));
            Assert.That(response2.Error, Is.EqualTo("Задание уже существует"));
        });
    }

    [Test]
    public async Task Test2()
    {
        using var scope = ScopeFactory.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService<InitialVerificationJobsController>();
        var result = await controller.AddJob(new(2023, 02));
        Assert.That(result.Error, Is.EqualTo("Год от 2024 до текущего"));
    }

    [Test]
    public async Task Test3()
    {
        using var scope = ScopeFactory.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService<InitialVerificationJobsController>();
        var response = await controller.GetJobs(new());
        Assert.That(response.Data!.Items, Has.Count.EqualTo(1));
    }
}
