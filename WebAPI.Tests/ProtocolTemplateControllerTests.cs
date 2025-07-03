using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database;
using WebAPI.Controllers;
using WebAPI.Controllers.Requests;

namespace WebAPI.Tests;

public class ProtocolTemplateControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        var scope = ScopeFactory.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService<ProtocolTemplateController>();
        var vmController = scope.ServiceProvider.GetRequiredService<VerificationMethodsController>();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();

        var fileName = "123.txt";
        var file = "file content".ContentToFormFile(fileName);
        var addVMResult = await vmController.AddVerificationMethod(new("Test1Desc", ["Test1"], fileName, file));
        var aliases = db.VerificationMethods.First().Aliases;

        var request = new AddProtocolTemplateRequest
        {
            DeviceTypeNumber = "deviceTypeNumber1",
            Group = "Манометр",
            Checkups = new Dictionary<string, string>() { { "Checkup1", "Checkup1Result" }, { "Checkup2", "Checkup2Result" } },
            Values = new Dictionary<string, object>() { { "ForwardValues", new double[] { 5, 6, 7 } }, { "BackwardValues", new double[] { 1, 2, 3 } } },
            VerificationMethodsAliases = [aliases]
        };

        var addTemplateResult = await controller.AddTemplate(request);

        Assert.Multiple(() =>
        {
            Assert.That(addVMResult.Message, Is.EqualTo("Метод поверки добавлен"));
            Assert.That(addTemplateResult.Message, Is.EqualTo("Протокол добавлен"));
        });
    }

    [Test]
    public async Task Test2()
    {
        var scope = ScopeFactory.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService<ProtocolTemplateController>();
        var result = await controller.GetTemplates(new());
        Assert.That(result.Data!.Items, Has.Count.EqualTo(1));
    }
}
