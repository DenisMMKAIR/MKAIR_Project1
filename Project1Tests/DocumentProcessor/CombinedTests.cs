namespace Project1Tests.DocumentProcessor;

internal class CombinedTests : CombinedFixture
{
    [Test]
    public async Task Success_PDF_Manometr1Eta()
    {
        var data = DummyManometr1Data.ManometrData1Eta();
        var result = await ManometrCreator.CreateAsync(data);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HTMLContent, Is.Not.Null);
            Assert.That(result.Error, Is.Null);
        }
        var dirPath = "__PDFTests".GetProjectDirPath();
        Directory.CreateDirectory(dirPath);
        var filePath = Path.Combine(dirPath, "Manometr1Eta.pdf");
        var success = await PdfExporter.ExportAsync(result.HTMLContent!, filePath);
        Assert.That(success, Is.True);
    }

    [Test]
    public async Task Success_PDF_Manometr2Eta()
    {
        var data = DummyManometr1Data.ManometrData2Eta();
        var result = await ManometrCreator.CreateAsync(data);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HTMLContent, Is.Not.Null);
            Assert.That(result.Error, Is.Null);
        }
        var dirPath = "__PDFTests".GetProjectDirPath();
        Directory.CreateDirectory(dirPath);
        var filePath = Path.Combine(dirPath, "Manometr2Eta.pdf");
        var success = await PdfExporter.ExportAsync(result.HTMLContent!, filePath);
        Assert.That(success, Is.True);
    }

    [Test]
    public async Task Success_PDF_Davlenie1Eta()
    {
        var data = DummyDavlenie1Data.DavlenieData1Eta();
        var result = await DavlenieCreator.CreateAsync(data);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HTMLContent, Is.Not.Null);
            Assert.That(result.Error, Is.Null);
        }
        var dirPath = "__PDFTests".GetProjectDirPath();
        Directory.CreateDirectory(dirPath);
        var filePath = Path.Combine(dirPath, "Davlenie1Eta.pdf");
        var success = await PdfExporter.ExportAsync(result.HTMLContent!, filePath);
        Assert.That(success, Is.True);
    }

    [Test]
    public async Task Success_PDF_Davlenie2Eta()
    {
        var data = DummyDavlenie1Data.DavlenieData2Eta();
        var result = await DavlenieCreator.CreateAsync(data);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HTMLContent, Is.Not.Null);
            Assert.That(result.Error, Is.Null);
        }
        var dirPath = "__PDFTests".GetProjectDirPath();
        Directory.CreateDirectory(dirPath);
        var filePath = Path.Combine(dirPath, "Davlenie2Eta.pdf");
        var success = await PdfExporter.ExportAsync(result.HTMLContent!, filePath);
        Assert.That(success, Is.True);
    }
}
