using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Entities;
using WebAPI.Controllers;

namespace WebAPI.Tests.DatabaseTemp;

public class InitialVerificationsControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        using var scope = ScopeFactory.CreateScope();
        var job = new InitialVerificationJob { Date = (2024, 02) };
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
        db.InitialVerificationJobs.Add(job);
        db.SaveChanges();
        var bs = scope.ServiceProvider.GetRequiredService<InitialVerificationBackgroundService>();
        var method = typeof(InitialVerificationBackgroundService)
            .GetMethod("ProcessWorkAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
        await (Task)method.Invoke(bs, null)!;

        var controller = scope.ServiceProvider.GetRequiredService<VerificationsController>();
        var response = await controller.GetInitialVerifications(new(), new());

        var deviceTypes = await db.DeviceTypes.CountAsync();
        var devices = await db.Devices.CountAsync();
        var eiv = await db.Etalons.SelectMany(e => e.SuccessInitialVerifications!).CountAsync();
        var eivf = await db.Etalons.SelectMany(e => e.FailedInitialVerifications!).CountAsync();
        var etalons = await db.Etalons.CountAsync();
        var fiv = await db.FailedInitialVerifications.CountAsync();
        var iv = await db.SuccessInitialVerifications.CountAsync();

        var valid = db.Devices.Count(d => d.DeviceType == null) +
                    db.SuccessInitialVerifications.Count(i => i.Device == null) +
                    db.FailedInitialVerifications.Count(f => f.Device == null) +
                    db.SuccessInitialVerifications.Count(i => i.Etalons!.Count == 0) +
                    db.FailedInitialVerifications.Count(i => i.Etalons!.Count == 0);

        Assert.Multiple(() =>
        {
            Assert.That(response.Data!.TotalCount, Is.EqualTo(689));
            Assert.That(deviceTypes, Is.EqualTo(66));
            Assert.That(devices, Is.EqualTo(683));
            Assert.That(eiv, Is.EqualTo(780));
            Assert.That(eivf, Is.EqualTo(0));
            Assert.That(etalons, Is.EqualTo(10));
            Assert.That(fiv, Is.EqualTo(0));
            Assert.That(iv, Is.EqualTo(689));
            Assert.That(valid, Is.EqualTo(0));
        });
    }
}
