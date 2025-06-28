namespace Project1Tests.FGISAPITests;

public class GetInitialVerificationsTests : FGISAPIClientFixture
{
    [Test]
    public async Task Test1()
    {
        var result = await Client.GetInitialVerifications(new DateOnly(2024, 2, 1));
        Assert.That(result, Has.Count.EqualTo(689));
    }
}
