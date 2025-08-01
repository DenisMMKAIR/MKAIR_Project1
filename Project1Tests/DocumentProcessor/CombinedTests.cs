using Infrastructure.DocumentProcessor.Creator;

namespace Project1Tests.DocumentProcessor;

internal class CombinedTests : CombinedFixture
{
    [Test]
    public async Task Success_PDF_Manometr2Eta()
    {
        var data = DummyManometr1Data.ManometrData2Eta();
        var result = await ManometrCreator.CreateAsync(data);
        await PDFTest(result, "Manometr2Eta.pdf");
    }

    [Test]
    public async Task Success_PDF_Davlenie2Eta()
    {
        var data = DummyDavlenie1Data.DavlenieData2Eta();
        var result = await DavlenieCreator.CreateAsync(data);
        await PDFTest(result, "Davlenie2Eta.pdf");
    }

    [Test]
    public async Task Success_PDF_Davlenie2Eta2()
    {
        var data = DummyDavlenie1Data.DavlenieData2Eta2();
        var result = await DavlenieCreator.CreateAsync(data);
        await PDFTest(result, "Davlenie2Eta2.pdf");
    }

    private async Task PDFTest(HTMLCreationResult result, string fileName)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HTMLContent, Is.Not.Null);
            Assert.That(result.Error, Is.Null);
        }
        var dirPath = "__PDFTests".GetProjectDirPath();
        Directory.CreateDirectory(dirPath);
        var filePath = Path.Combine(dirPath, fileName);
        var success = await PdfExporter.ExportAsync(result.HTMLContent!, filePath);
        Assert.That(success, Is.True);
    }
}
