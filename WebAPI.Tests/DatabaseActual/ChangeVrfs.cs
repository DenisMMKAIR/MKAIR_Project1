using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;

namespace WebAPI.Tests.DatabaseActual;

public class ChangeVrfs : DatabaseActualFixture
{
    // [Test]
    public async Task Test1()
    {
        using var scope = ScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();

        var sIvDups = await CountDuplicatesAsync(db.SuccessInitialVerifications);
        var fIvDups = await CountDuplicatesAsync(db.FailedInitialVerifications);
        var manDups = await CountDuplicatesAsync(db.Manometr1Verifications);
        var davlDups = await CountDuplicatesAsync(db.Davlenie1Verifications);

        Assert.Multiple(() =>
        {
            Assert.That(sIvDups, Is.EqualTo(0), message: "SuccessInitialVerifications has duplicates");
            Assert.That(fIvDups, Is.EqualTo(0), message: "FailedInitialVerifications has duplicates");
            Assert.That(manDups, Is.EqualTo(0), message: "Manometr1Verifications has duplicates");
            Assert.That(davlDups, Is.EqualTo(0), message: "Davlenie1Verifications has duplicates");
        });
    }

    // [Test]
    public async Task Test2()
    {
        using var scope = ScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();

        var vrfs = await db.Davlenie1Verifications.ToArrayAsync();

        vrfs.GroupBy(vrf => vrf, VerificationUniqComparer.Instance)
            .Where(g => g.Count() > 1)
            .ToList()
            .ForEach(g =>
            {
                var toRemove = g.Skip(1).ToArray();
                db.RemoveRange(toRemove);
            });

        var changed = await db.SaveChangesAsync();

        Assert.That(changed, Is.Not.EqualTo(0));
    }

    private static async Task<int> CountDuplicatesAsync<T>(IQueryable<T> query) where T : IVerificationBase
    {
        IReadOnlyList<T> vrfs = await query.ToArrayAsync();
        return vrfs.GroupBy(vrf => vrf, VerificationUniqComparer.Instance).Count(g => g.Count() > 1);
    }
}