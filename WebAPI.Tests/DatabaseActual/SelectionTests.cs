using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database;

namespace WebAPI.Tests.DatabaseActual;

public class SelectionTests : DatabaseActualFixture
{
    // [Test]
    public async Task Test1()
    {
        using var scope = ScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
        var ivs = await db.SuccessInitialVerifications
            .Where(iv => iv.VerificationDate.Year == 2025 && iv.VerificationDate.Month == 2)
            .GroupBy(iv => iv.Device!.DeviceType!.Title.Substring(0, iv.Device.DeviceType.Title.IndexOf(" ")))
            .Select(g => new { g.Key, Titles = g.Select(iv => iv.Device!.DeviceType!.Title).Distinct().Order().ToArray() })
            .OrderBy(g => g.Key)
            .ToArrayAsync();
    }
}
