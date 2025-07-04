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
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
        var controller = scope.ServiceProvider.GetRequiredService<ProtocolTemplateController>();
        var vmController = scope.ServiceProvider.GetRequiredService<VerificationMethodsController>();

        var addVMResult = await vmController.AddVerificationMethod(CreateVerificationMethod(["Test1"]));
        var vmIds = db.VerificationMethods.Select(vm => vm.Id).ToArray();
        var addTemplateResult1 = await controller.AddTemplate(CreateTemplate(vmIds));
        var addTemplateResult2 = await controller.AddTemplate(CreateTemplate(vmIds));

        Assert.Multiple(() =>
        {
            Assert.That(addVMResult.Message, Is.EqualTo("Метод поверки добавлен"));
            Assert.That(addTemplateResult1.Message, Is.EqualTo("Протокол добавлен"));
            Assert.That(addTemplateResult2.Error, Is.EqualTo("Протокол с номером типа устройства существует"));
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

    private static AddVerificationMethodRequest CreateVerificationMethod(string[] aliases)
    {
        var fileName = "123.txt";
        var file = "file content".ContentToFormFile(fileName);
        return new AddVerificationMethodRequest("Test1Desc", aliases, fileName, file);
    }

    private static AddProtocolTemplateRequest CreateTemplate(Guid[] vmIds) => new()
    {
        VerificationMethodsIds = vmIds,
        DeviceTypeNumber = "deviceTypeNumber1",
        Group = "Манометр",
        Checkups = new Dictionary<string, string>() { { "Checkup1", "Checkup1Result" }, { "Checkup2", "Checkup2Result" } },
        Values = new Dictionary<string, object>() { { "ForwardValues", new double[] { 5, 6, 7 } }, { "BackwardValues", new double[] { 1, 2, 3 } } },
    };
}
