using ProjApp.ProtocolForms;

namespace Project1Tests.DocumentProcessor;

internal class ExportPDFTests : CombinedFixture
{
    [Test]
    public async Task Manometr_ShortDeviceInfo()
    {
        await PDFTest(DummyManometr1Data.ShortDeviceInfo(), "ManometrShortDeviceInfo.pdf");
    }

    [Test]
    public async Task Manometr_LongDeviceInfo()
    {
        await PDFTest(DummyManometr1Data.LongDeviceInfo(), "ManometrLongDeviceInfo.pdf");
    }

    private async Task PDFTest(IProtocolForm data, string fileName)
    {
        var dirPath = "__PDFTests".GetProjectDirPath();
        Directory.CreateDirectory(dirPath);
        var filePath = Path.Combine(dirPath, fileName);
        var success = await Processor.CreatePDFAsync(data, filePath);
        Assert.That(success, Is.True);
    }
}
