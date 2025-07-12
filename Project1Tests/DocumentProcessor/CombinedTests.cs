namespace Project1Tests.DocumentProcessor;

internal class CombinedTests : CombinedFixture
{
    [Test]
    public async Task Success_PDF_Manometr1Eta()
    {
        var data = DummyDocumentData.ManometrData1Eta();
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
        var data = DummyDocumentData.ManometrData2Eta();
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
}
