using Infrastructure.DocumentProcessor.Creator;

namespace Project1Tests.DocumentProcessor;

internal class CombinedTests : CombinedFixture
{
    [Test]
    public async Task Manometr_ShortDeviceInfo()
    {
        var data = DummyManometr1Data.ShortDeviceInfo();
        var result = await ManometrCreator.CreateAsync(data);
        await PDFTest(result, "ManometrShortDeviceInfo.pdf");
    }

    [Test]
    public async Task Manometr_LongDeviceInfo()
    {
        var data = DummyManometr1Data.LongDeviceInfo();
        var result = await ManometrCreator.CreateAsync(data);
        await PDFTest(result, "ManometrLongDeviceInfo.pdf");
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
