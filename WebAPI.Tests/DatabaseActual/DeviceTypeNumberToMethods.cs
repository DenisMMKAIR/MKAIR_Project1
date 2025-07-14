using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database;

namespace WebAPI.Tests.DatabaseActual;

public class DeviceTypeNumberToMethods : DatabaseActualFixture
{
    [Test]
    public async Task Test1()
    {
        Environment.Exit(0);

        await using var scope = ScopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();

        var dtMethods = db.SuccessInitialVerifications
            .AsEnumerable()
            .Select(v => new
            {
                Number = v.DeviceTypeNumber,
                Aliases = new[] { v.VerificationTypeName },
            })
            .Union(db.FailedInitialVerifications
                .Select(v => new
                {
                    Number = v.DeviceTypeNumber,
                    Aliases = new[] { v.VerificationTypeName },
                }))
            .Union(db.DeviceTypes
                .Where(dt => dt.VerificationMethod != null)
                .Select(dt => new
                {
                    dt.Number,
                    Aliases = dt.VerificationMethod!.Aliases.ToArray(),
                }))
            .GroupBy(g => g.Number)
            .Select(g => new
            {
                Number = g.Key,
                Aliases = g.SelectMany(m => m.Aliases)
                    .DistinctBy(a => a.Replace(" ", "").Trim())
                    .OrderBy(a => a.Length)
                    .ToArray(),
            })
            .OrderByDescending(dto => dto.Aliases.Length)
            .ToArray();

        using var fs = File.OpenWrite(
            Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop),
                "методики_по_номеру_типа.txt"));

        using var sw = new StreamWriter(fs);

        foreach (var dto in dtMethods)
        {
            sw.WriteLine($"{dto.Number}\n\t{string.Join("\n\t", dto.Aliases)}");
        }
    }
}
