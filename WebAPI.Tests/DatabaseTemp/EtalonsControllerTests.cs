using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database;
using ProjApp.Database.Entities;

namespace WebAPI.Tests.DatabaseTemp;

public class EtalonsControllerTests : ControllersFixture
{
    [Test]
    public async Task Test1()
    {
        {
            using var scope = ScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();

            var etalon = CreateEtalon("1");
            var verifications = new[] {
                CreateSuccessInitialVerification("1", etalon),
                CreateSuccessInitialVerification("2", etalon)
            };
            db.AddRange(verifications);

            var etalons = new[] { CreateEtalon("2"), CreateEtalon("3") };
            var verification = CreateSuccessInitialVerification("3", etalons);
            db.Add(verification);

            await db.SaveChangesAsync();
        }
        {
            using var scope = ScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
            var etalon = db.Etalons
                .Include(e => e.SuccessInitialVerifications)
                .First(e => e.Number == "1");

            var verification = db.SuccessInitialVerifications
                .Include(v => v.Etalons)
                .First(v => v.DeviceTypeNumber == "3");

            Assert.Multiple(() =>
            {
                Assert.That(etalon.SuccessInitialVerifications, Has.Count.EqualTo(2));
                Assert.That(verification.Etalons, Has.Count.EqualTo(2));
            });
        }
    }

    private static Etalon CreateEtalon(string number)
    {
        return new()
        {
            Number = number,
            Date = default,
            ToDate = default,
            FullInfo = "",
        };
    }

    private static SuccessInitialVerification CreateSuccessInitialVerification(string number, params Etalon[] etalons)
    {
        return new()
        {
            DeviceTypeNumber = number,
            AdditionalInfo = [],
            DeviceSerial = "",
            Owner = "",
            VerificationDate = default,
            VerificationTypeName = "",
            VerifiedUntilDate = default,

            Etalons = etalons,
        };
    }
}
