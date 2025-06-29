using ProjApp.Database.Commands;

namespace Project1Tests.FGISAPITests;

public class GetInitialVerificationsTests : FGISAPIFixture
{
    [Test]
    public async Task Test1()
    {
        var result = await Client.GetInitialVerifications(new DateOnly(2024, 2, 1));
        var etalons = result.SelectMany(v => v.Etalons!).Distinct(new EtalonUniqComparer()).ToArray();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(etalons, Has.Length.EqualTo(32));
            Assert.That(result, Has.Count.EqualTo(689));
        }
    }
}
