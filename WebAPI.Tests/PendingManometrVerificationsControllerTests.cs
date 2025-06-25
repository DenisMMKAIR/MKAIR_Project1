using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database;
using WebAPI.Controllers;

namespace WebAPI.Tests;

public class PendingManometrVerificationsControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        var verifications1File = Samples.Verifications1.ToFormFile();
        var controller = ServiceProvider.GetRequiredService<PendingManometrVerificationsController>();
        var response = await controller.AcceptExcelVerifications(new(verifications1File, "excel", "A1:P555", DeviceLocation.АнтипинскийНПЗ), CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(response.Message, Is.EqualTo("Добавлено 554 записей. Отсеясно дубликатов 0"));
            Assert.That(response.Error, Is.Null);
        });
    }

    [Test]
    public async Task Test2()
    {
        var verifications1File = Samples.Verifications1.ToFormFile();
        var controller = ServiceProvider.GetRequiredService<PendingManometrVerificationsController>();
        var response = await controller.AcceptExcelVerifications(new(verifications1File, "excel", "A1:P555", DeviceLocation.АнтипинскийНПЗ), CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(response.Message, Is.EqualTo("Добавлено 0 записей. Отсеясно дубликатов 554"));
            Assert.That(response.Error, Is.Null);
        });
    }

    [Test]
    public async Task Test3()
    {
        var controller = ServiceProvider.GetRequiredService<PendingManometrVerificationsController>();
        var response = await controller.GetPandingVerificationsPaginated(new());
        Assert.Multiple(() =>
        {
            Assert.That(response.Error, Is.Null);
            Assert.That(response.Data, Is.Not.Null);
        });
        Assert.That(response.Data.TotalCount, Is.EqualTo(554));
    }

    [Test]
    public async Task Test4()
    {
        var controller = ServiceProvider.GetRequiredService<PendingManometrVerificationsController>();
        var response = await controller.GetPandingVerificationsPaginated(new(1, 100));
        Assert.Multiple(() =>
        {
            Assert.That(response.Error, Is.Null);
            Assert.That(response.Data, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(response.Data.TotalCount, Is.EqualTo(554));
            Assert.That(response.Data.TotalPages, Is.EqualTo(6));
            Assert.That(response.Data.Items, Has.Count.EqualTo(100));
        });
    }
}