using ProjApp.Database.Commands;

namespace Project1Tests.FGISAPITests;

public class GetInitialVerificationsTests : FGISAPIFixture
{
    [Test]
    public async Task Test1()
    {
        var (good, failed) = await Client.GetInitialVerifications(new DateOnly(2024, 2, 1));

        var etalons = good
            .SelectMany(v => v.Etalons!)
            .Concat(failed.SelectMany(v => v.Etalons!))
            .Distinct(new EtalonUniqComparer())
            .ToArray();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(etalons, Has.Length.EqualTo(32));
            Assert.That(good, Has.Count.EqualTo(689));
            Assert.That(failed, Has.Count.EqualTo(0));
        }
    }
}
