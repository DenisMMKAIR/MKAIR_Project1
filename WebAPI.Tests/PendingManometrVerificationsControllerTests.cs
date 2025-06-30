using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database;
using WebAPI.Controllers;
using WebAPI.Controllers.Requests;

namespace WebAPI.Tests;

public class PendingManometrVerificationsControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        var controller = ServiceProvider.GetRequiredService<PendingManometrVerificationsController>();
        var verificationsFile = Samples.Verifications1.ToFormFile();
        var request = new ExcelVerificationsRequest(verificationsFile, "excel", "A1:P555", DeviceLocation.АнтипинскийНПЗ);

        var response1 = await controller.AcceptExcelVerifications(request, CancellationToken.None);
        var response2 = await controller.AcceptExcelVerifications(request, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response1.Message, Is.EqualTo("Добавлено новых элементов 554. Отсеяно дубликатов 0"));
            Assert.That(response2.Message, Is.EqualTo("Все записи уже существуют в базе данных"));
        });
    }

    [Test]
    public async Task Test2()
    {
        var controller = ServiceProvider.GetRequiredService<PendingManometrVerificationsController>();
        var response = await controller.GetPandingVerificationsPaginated(new());
        Assert.That(response.Data!.TotalCount, Is.EqualTo(554));
    }

    [Test]
    public async Task Test3()
    {
        var controller = ServiceProvider.GetRequiredService<PendingManometrVerificationsController>();
        var response = await controller.GetPandingVerificationsPaginated(new(1, 100));
        Assert.Multiple(() =>
        {
            Assert.That(response.Data!.TotalCount, Is.EqualTo(554));
            Assert.That(response.Data.TotalPages, Is.EqualTo(6));
            Assert.That(response.Data.Items, Has.Count.EqualTo(100));
        });
    }
}