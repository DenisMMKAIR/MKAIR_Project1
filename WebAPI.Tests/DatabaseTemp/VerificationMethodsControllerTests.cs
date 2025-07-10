// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using ProjApp.Database;
// using WebAPI.Controllers;
// using WebAPI.Controllers.Requests;

// namespace WebAPI.Tests.DatabaseTemp;

// public class VerificationMethodsControllerTests : ControllersFixture
// {
//     [Test]
//     public async Task Test1()
//     {
//         using var scope = ScopeFactory.CreateScope();
//         var controller = scope.ServiceProvider.GetRequiredService<VerificationMethodsController>();
//         var fileName = "test_file.txt";
//         var file = "test content".ContentToFormFile(fileName);

//         var newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1_1"), new("test1_2")], fileName, file);
//         var result1 = await controller.AddVerificationMethod(newVrfMethod);

//         newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1_3"), new("test1_3")], fileName, file);
//         var result2 = await controller.AddVerificationMethod(newVrfMethod);

//         newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1_2")], fileName, file);
//         var result3 = await controller.AddVerificationMethod(newVrfMethod);

//         newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test1_4"), new("test1_1")], fileName, file);
//         var result4 = await controller.AddVerificationMethod(newVrfMethod);

//         var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
//         var dbFile = await db.VerificationMethodFiles.SingleOrDefaultAsync(file => file.FileName == fileName);

//         Assert.Multiple(() =>
//         {
//             Assert.That(result1.Message, Is.EqualTo("Метод поверки добавлен"));
//             Assert.That(result2.Error, Is.EqualTo("Псевдонимы должны быть уникальными"));
//             Assert.That(result3.Error, Is.EqualTo("Метод поверки с псевдонимом уже существует"));
//             Assert.That(result4.Error, Is.EqualTo("Метод поверки с псевдонимом уже существует"));
//             Assert.That(dbFile, Is.Not.Null);
//         });
//     }

//     [Test]
//     public async Task Test2()
//     {
//         using var scope = ScopeFactory.CreateScope();
//         var controller = scope.ServiceProvider.GetRequiredService<VerificationMethodsController>();
//         var fileName = "test_file2.txt";
//         var file = "test content".ContentToFormFile(fileName);

//         var newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test2_1")], fileName, file);
//         var result1 = await controller.AddVerificationMethod(newVrfMethod);

//         newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test2_2")], "", file);
//         var result2 = await controller.AddVerificationMethod(newVrfMethod);

//         newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test2_3")], "xt", file);
//         var result3 = await controller.AddVerificationMethod(newVrfMethod);

//         var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
//         var dbFile = await db.VerificationMethodFiles.SingleOrDefaultAsync(file => file.FileName == fileName);

//         Assert.Multiple(() =>
//         {
//             Assert.That(result1.Message, Is.EqualTo("Метод поверки добавлен"));
//             Assert.That(result2.Error, Is.EqualTo("Некорректное имя файла"));
//             Assert.That(result3.Error, Is.EqualTo("Некорректное имя файла"));
//             Assert.That(dbFile, Is.Not.Null);
//         });
//     }

//     [Test]
//     public async Task Test3()
//     {
//         using var scope = ScopeFactory.CreateScope();
//         var controller = scope.ServiceProvider.GetRequiredService<VerificationMethodsController>();
//         var fileName = "test_file3.txt";

//         var file1 = "test content".ContentToFormFile(fileName);
//         var newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test3_1")], fileName, file1);
//         var result1 = await controller.AddVerificationMethod(newVrfMethod);

//         var file2 = "".ContentToFormFile(fileName);
//         newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test3_1")], fileName, file2);
//         var result2 = await controller.AddVerificationMethod(newVrfMethod);

//         var file3 = "1d".ContentToFormFile(fileName);
//         newVrfMethod = new AddVerificationMethodRequest("test desc", [new("test3_1")], fileName, file3);
//         var result3 = await controller.AddVerificationMethod(newVrfMethod);

//         var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
//         var dbFile = await db.VerificationMethodFiles.SingleOrDefaultAsync(file => file.FileName == fileName);

//         Assert.Multiple(() =>
//         {
//             Assert.That(result1.Message, Is.EqualTo("Метод поверки добавлен"));
//             Assert.That(result2.Error, Is.EqualTo("Файл пустой или некорректный"));
//             Assert.That(result3.Error, Is.EqualTo("Файл пустой или некорректный"));
//             Assert.That(dbFile, Is.Not.Null);
//         });
//     }

//     [Test]
//     public async Task Test4()
//     {
//         using var scope = ScopeFactory.CreateScope();
//         var controller = scope.ServiceProvider.GetRequiredService<VerificationMethodsController>();
//         var fileName = "test_file4.txt";
//         var file = "test content".ContentToFormFile(fileName);

//         var newVrfMethod = new AddVerificationMethodRequest("test desc", [new("\"«»test4_1")], fileName, file);
//         var result1 = await controller.AddVerificationMethod(newVrfMethod);

//         var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
//         var alias = await db.VerificationMethods
//                             .SelectMany(vm => vm.Aliases)
//                             .SingleOrDefaultAsync(alias => alias == "TEST4_1");
//         var dbFile = await db.VerificationMethodFiles.SingleOrDefaultAsync(file => file.FileName == fileName);

//         Assert.Multiple(() =>
//         {
//             Assert.That(result1.Message, Is.EqualTo("Метод поверки добавлен"));
//             Assert.That(alias, Is.Not.Null);
//             Assert.That(dbFile, Is.Not.Null);
//         });
//     }

//     [Test]
//     public async Task Test5()
//     {
//         using var scope = ScopeFactory.CreateScope();
//         var controller = scope.ServiceProvider.GetRequiredService<VerificationMethodsController>();
//         var vmResult = await controller.GetVerificationMethods(new());
//         var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
//         var filesResult = await db.VerificationMethodFiles.CountAsync();
//         Assert.Multiple(() =>
//         {
//             Assert.That(vmResult.Data!.Items, Has.Count.EqualTo(4));
//             Assert.That(filesResult, Is.EqualTo(4));
//         });
//     }
// }
