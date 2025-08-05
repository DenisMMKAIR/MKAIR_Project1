using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database;
using ProjApp.Normalizers.VerificationMethod;
using ProjApp.Services;

namespace WebAPI.Tests.DatabaseActual;

public class ChangeVerificationMethod : DatabaseActualFixture
{
    // [Test]
    public async Task Test1()
    {
        using var scope = ScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();

        var vm = await db.VerificationMethods
            .SingleAsync(m => m.Description.Contains("EJX"));

        var c = VerificationMethodAliasComparerNormalizer.Instance.Normalize;
        var n = VerificationMethodAliasVisualNormalizer.Instance.Normalize;

        var aliases = new string[] {
            "ГСИ. ПРЕОБРАЗОВАТЕЛИ ДАВЛЕНИЯ ИЗМЕРИТЕЛЬНЫЕ EJX. МЕТОДИКА ПОВЕРКИ",
            "ГЦИ. ПРЕОБРАЗОВАТЕЛИ ДАВЛЕНИЯ ИЗМЕРИТЕЛЬНЫЕ EJX. МЕТОДИКА ПОВЕРКИ",
            """ГЦИ. Преобразователи давления измерительные EJX. Методика поверки""",
            "ПРЕОБРАЗОВАТЕЛИ ДАВЛЕНИЯ ИЗМЕРИТЕЛЬНЫЕ EJX. МЕТОДИКА ПОВЕРКИ.",
            """Преобразователи давления измерительные EJX. Методика поверки."""
        }
            .DistinctBy(c)
            .Select(n)
            .ToArray();

        vm.Aliases = aliases.Take(2).ToArray();
        await db.SaveChangesAsync();

        var vmService = scope.ServiceProvider.GetRequiredService<VerificationMethodsService>();
        await vmService.AddAliasesAsync(aliases.Skip(2).ToArray(), vm.Id);
    }

    // [Test]
    // public async Task Test2()
    // {
    //     using var scope = ScopeFactory.CreateScope();
    //     var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();

    //     foreach (var vm in db.VerificationMethods)
    //     {
    //         vm.CheckupsNew = [];
    //         foreach (var (key, value) in vm.Checkups)
    //         {
    //             CheckupType.ChType t;
    //             if (key.StartsWith("Опред"))
    //                 t = CheckupType.ChType.Fact;
    //             else t = CheckupType.ChType.Yes_No;

    //             vm.CheckupsNew[key] = new() { Type = t, Value = value };
    //         }
    //     }

    //     await db.SaveChangesAsync();
    // }
}