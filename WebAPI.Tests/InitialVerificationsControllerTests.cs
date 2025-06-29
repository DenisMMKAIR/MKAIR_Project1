using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Entities;
using WebAPI.Controllers;

namespace WebAPI.Tests;

public class InitialVerificationsControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        var job = new InitialVerificationJob { Date = (2024, 02) };
        var db = ServiceProvider.GetRequiredService<ProjDatabase>();
        db.InitialVerificationJobs.Add(job);
        db.SaveChanges();
        var bs = ServiceProvider.GetRequiredService<InitialVerificationBackgroundService>();
        var method = typeof(InitialVerificationBackgroundService)
            .GetMethod("ProcessWorkAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
        await (Task)method.Invoke(bs, null)!;

        var controller = ServiceProvider.GetRequiredService<InitialVerificationsController>();
        var response = await controller.GetVerifications(new());
        Assert.That(response.Data, Is.Not.Null);
        Assert.That(response.Data.Items, Has.Count.EqualTo(10));
    }
}
