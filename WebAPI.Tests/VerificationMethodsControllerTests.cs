using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database;
using WebAPI.Controllers;
using WebAPI.Controllers.Requests;

namespace WebAPI.Tests;

public class VerificationMethodsControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        var db = ServiceProvider.GetRequiredService<ProjDatabase>();

        var existsMethod = db.VerificationMethods.FirstOrDefault(vm => vm.Name == "test");
        if (existsMethod != null)
        {
            db.VerificationMethods.Remove(existsMethod);
            await db.SaveChangesAsync();
        }

        var controller = ServiceProvider.GetRequiredService<VerificationMethodsController>();
        var newVrfMethod = new AddVerificationMethodRequest("test", "test desc", [new("test"), new("test1")], [new("test")]);
        var result = await controller.AddVerificationMethod(newVrfMethod);
        Assert.That(result.Message, Is.EqualTo("Метод поверки добавлен"));

        newVrfMethod = new AddVerificationMethodRequest("test", "test desc", [new("test")], [new("test")]);
        result = await controller.AddVerificationMethod(newVrfMethod);
        Assert.That(result.Message, Is.EqualTo("Такой метод поверки уже существует"));

        newVrfMethod = new AddVerificationMethodRequest("test1", "test desc", [new("test1"), new("test2")], [new("test")]);
        result = await controller.AddVerificationMethod(newVrfMethod);
        Assert.That(result.Message, Is.EqualTo("Такой метод поверки уже существует"));
    }

    [Test]
    public async Task Test2()
    {
        var controller = ServiceProvider.GetRequiredService<VerificationMethodsController>();
        var result = await controller.GetVerificationMethods(new());
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Items, Has.Count.EqualTo(1));
    }
}
