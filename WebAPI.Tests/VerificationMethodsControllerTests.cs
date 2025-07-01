using System.Text;
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
        var fileName = "test_file.txt";
        var fileContent = Encoding.UTF8.GetBytes("test content");

        var newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1_1"), new("test1_2")], fileName, fileContent);
        var result1 = await controller.AddVerificationMethod(newVrfMethod);

        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1_3"), new("test1_3")], fileName, fileContent);
        var result2 = await controller.AddVerificationMethod(newVrfMethod);

        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1_2")], fileName, fileContent);
        var result3 = await controller.AddVerificationMethod(newVrfMethod);

        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1_4"), new("test1_1")], fileName, fileContent);
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
        var fileContent = Encoding.UTF8.GetBytes("test content");
        
        var newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test2_1")], "test_file.txt", fileContent);
        var result1 = await controller.AddVerificationMethod(newVrfMethod);

        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test2_1")], "", fileContent);
        var result2 = await controller.AddVerificationMethod(newVrfMethod);

        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test2_1")], "xt", fileContent);
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

        var fileContent1 = Encoding.UTF8.GetBytes("test content");
        var newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test3_1")], fileName, fileContent1);
        var result1 = await controller.AddVerificationMethod(newVrfMethod);

        var fileContent2 = Encoding.UTF8.GetBytes("");
        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test3_1")], fileName, fileContent2);
        var result2 = await controller.AddVerificationMethod(newVrfMethod);

        var fileContent3 = Encoding.UTF8.GetBytes("1d");
        newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test3_1")], fileName, fileContent3);
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
        var result = await controller.GetVerificationMethods(new());
        Assert.That(result.Data!.Items, Has.Count.EqualTo(3));
    }
}
