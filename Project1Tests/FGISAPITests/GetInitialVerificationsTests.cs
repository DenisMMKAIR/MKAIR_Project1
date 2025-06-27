namespace Project1Tests.FGISAPITests;

public class GetInitialVerificationsTests : FGISAPIClientFixture
{
    [Test]
    public async Task Test1()
    {
        var result = await Client.GetInitialVerifications("2024.01");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.InitialVerifications, Has.Count.EqualTo(1));
            Assert.That(result.Etalons, Has.Count.EqualTo(1));
        }
    }
}
