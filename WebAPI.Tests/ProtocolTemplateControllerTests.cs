using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
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
        var result = await vmController.AddVerificationMethod(new("Test1",["Test1"],));
        var request = new AddProtocolTemplateRequest(
            "deviceTypeNumber1",
            "Манометр",
            [new() { Name = "Checkup1", Value = "Checkup1Result", }, new() { Name = "Checkup2", Value = "Checkup2Result" }],
            new Dictionary<string, object>() { { "ForwardValues", new double[] { 5, 6, 7 } }, { "BackwardValues", new double[] { 1, 2, 3 } } },
            []);
        var result = await controller.AddTemplate(request);
        Assert.That(result.Message, Is.EqualTo("Протокол добавлен"));
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
