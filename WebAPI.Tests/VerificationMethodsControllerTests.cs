using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database;
using ProjApp.Database.Entities;
using WebAPI.Controllers;
using WebAPI.Controllers.Requests;

namespace WebAPI.Tests;

public class VerificationMethodsControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        var controller = ServiceProvider.GetRequiredService<VerificationMethodsController>();
        var fileName = "test_file.txt";
        var file = "test content".ContentToFormFile(fileName);

        var newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1_1"), new("test1_2")], fileName, file);
        var result1 = await controller.AddVerificationMethod(newVrfMethod);

        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1_3"), new("test1_3")], fileName, file);
        var result2 = await controller.AddVerificationMethod(newVrfMethod);

        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1_2")], fileName, file);
        var result3 = await controller.AddVerificationMethod(newVrfMethod);

        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1_4"), new("test1_1")], fileName, file);
        var result4 = await controller.AddVerificationMethod(newVrfMethod);

        Assert.Multiple(() =>
        {
            Assert.That(result1.Message, Is.EqualTo("Метод поверки добавлен"));
            Assert.That(result2.Error, Is.EqualTo("Псевдонимы должны быть уникальными"));
            Assert.That(result3.Error, Is.EqualTo("Метод поверки с псевдонимом уже существует"));
            Assert.That(result4.Error, Is.EqualTo("Метод поверки с псевдонимом уже существует"));
        });
    }

    [Test]
    public async Task Test2()
    {
        var controller = ServiceProvider.GetRequiredService<VerificationMethodsController>();
        var file = "test content".ContentToFormFile("test_file.txt");

        var newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test2_1")], "test_file.txt", file);
        var result1 = await controller.AddVerificationMethod(newVrfMethod);

        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test2_1")], "", file);
        var result2 = await controller.AddVerificationMethod(newVrfMethod);

        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test2_1")], "xt", file);
        var result3 = await controller.AddVerificationMethod(newVrfMethod);

        Assert.Multiple(() =>
        {
            Assert.That(result1.Message, Is.EqualTo("Метод поверки добавлен"));
            Assert.That(result2.Error, Is.EqualTo("Некорректное имя файла"));
            Assert.That(result3.Error, Is.EqualTo("Некорректное имя файла"));
        });
    }

    [Test]
    public async Task Test3()
    {
        var controller = ServiceProvider.GetRequiredService<VerificationMethodsController>();
        var fileName = "test_file.txt";

        var file1 = "test content".ContentToFormFile(fileName);
        var newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test3_1")], fileName, file1);
        var result1 = await controller.AddVerificationMethod(newVrfMethod);

        var file2 = "".ContentToFormFile(fileName);
        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test3_1")], fileName, file2);
        var result2 = await controller.AddVerificationMethod(newVrfMethod);

        var file3 = "1d".ContentToFormFile(fileName);
        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test3_1")], fileName, file3);
        var result3 = await controller.AddVerificationMethod(newVrfMethod);

        Assert.Multiple(() =>
        {
            Assert.That(result1.Message, Is.EqualTo("Метод поверки добавлен"));
            Assert.That(result2.Error, Is.EqualTo("Файл пустой или некорректный"));
            Assert.That(result3.Error, Is.EqualTo("Файл пустой или некорректный"));
        });
    }

    [Test]
    public async Task Test4()
    {
        var controller = ServiceProvider.GetRequiredService<VerificationMethodsController>();
        var fileName = "test_file.txt";
        var file = "test content".ContentToFormFile(fileName);

        var newVrfMethod = new AddVerificationMethodRequest("test desc", [new("\"«»test4_1")], fileName, file);
        var result1 = await controller.AddVerificationMethod(newVrfMethod);

        var db = ServiceProvider.GetRequiredService<ProjDatabase>();
        var alias = await db.Set<VerificationMethodAlias>().SingleOrDefaultAsync(x => x.Name == "TEST4_1");

        Assert.Multiple(() =>
        {
            Assert.That(result1.Message, Is.EqualTo("Метод поверки добавлен"));
            Assert.That(alias, Is.Not.Null);
        });
    }

    [Test]
    public async Task Test5()
    {
        var controller = ServiceProvider.GetRequiredService<VerificationMethodsController>();
        var result = await controller.GetVerificationMethods(new());
        Assert.That(result.Data!.Items, Has.Count.EqualTo(4));
    }
}
