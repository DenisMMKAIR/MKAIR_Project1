namespace Project1Tests.DocumentProcessor;

internal class CombinedTests : CombinedFixture
{
    [Test]
    public async Task Success_PDF_Manometr1Ver1Eta()
    {
        var data = DummyDocumentData.ManometrData1Ver1Eta();
        var result = await ManometrCreator.CreateAsync(data);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HTMLContent, Is.Not.Null);
            Assert.That(result.Error, Is.Null, result.Error);
        }
        var dirPath = "__PDFTests".GetProjectDirPath();
        Directory.CreateDirectory(dirPath);
        var filePath = Path.Combine(dirPath, data.FileName);
        var success = await PdfExporter.ExportAsync(result.HTMLContent!, filePath);
        Assert.That(success, Is.True);
    }

    [Test]
    public async Task Success_PDF_Manometr2Ver2Eta()
    {
        var data = DummyDocumentData.ManometrData2Ver2Eta();
        var result = await ManometrCreator.CreateAsync(data);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HTMLContent, Is.Not.Null);
            Assert.That(result.Error, Is.Null, result.Error);
        }
        var dirPath = "__PDFTests".GetProjectDirPath();
        Directory.CreateDirectory(dirPath);
        var filePath = Path.Combine(dirPath, data.FileName);
        var success = await PdfExporter.ExportAsync(result.HTMLContent!, filePath);
        Assert.That(success, Is.True);
    }
}
