namespace Project1Tests.DocumentProcessor;

internal class HTMLManometrTests : CombinedFixture
{
    [Test]
    public async Task Test1()
    {
        var data = DummyManometr1Data.ManometrData1Eta();
        var result = await ManometrCreator.CreateAsync(data);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HTMLContent, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Error, Is.Null);
        }
    }
}
