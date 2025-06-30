using Microsoft.Extensions.DependencyInjection;
using WebAPI.Controllers;
using WebAPI.Controllers.Requests;

namespace WebAPI.Tests;

public class VerificationMethodsControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        var controller = ServiceProvider.GetRequiredService<VerificationMethodsController>();

        var newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test"), new("test1")], [new("test")]);
        var result1 = await controller.AddVerificationMethod(newVrfMethod);

        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test")], [new("test")]);
        var result2 = await controller.AddVerificationMethod(newVrfMethod);

        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1"), new("test2")], [new("test")]);
        var result3 = await controller.AddVerificationMethod(newVrfMethod);

        Assert.Multiple(() =>
        {
            Assert.That(result1.Message, Is.EqualTo("Метод поверки добавлен"));
            Assert.That(result2.Error, Is.EqualTo("Такой метод поверки уже существует"));
            Assert.That(result3.Error, Is.EqualTo("Такой метод поверки уже существует"));
        });
    }

    [Test]
    public async Task Test2()
    {
        var controller = ServiceProvider.GetRequiredService<VerificationMethodsController>();
        var result = await controller.GetVerificationMethods(new());
        Assert.That(result.Data!.Items, Has.Count.EqualTo(1));
    }
}
