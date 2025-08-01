using ProjApp.ProtocolForms;

namespace Project1Tests.DocumentProcessor;

internal class DavlenieExportTests : CombinedFixture
{
    [Test]
    public async Task Davlenie_Checkups_1()
    {
        await PDFTest(
            DummyDavlenie1Data.CreateForm(checkupsCount: 1),
            "DavlenieCheckups1.pdf");
    }

    [Test]
    public async Task Davlenie_Checkups_5()
    {
        await PDFTest(
            DummyDavlenie1Data.CreateForm(checkupsCount: 5),
            "DavlenieCheckups5.pdf");
    }

    private async Task PDFTest(IProtocolForm data, string fileName)
    {
        var dirPath = "__PDFTests".GetProjectDirPath();
        Directory.CreateDirectory(dirPath);
        var filePath = Path.Combine(dirPath, fileName);
        var exportResult = await Processor.CreatePDFAsync(data, filePath);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exportResult.Error, Is.Null);
            Assert.That(exportResult.Message, Is.EqualTo($"Файл протокола {fileName} создан"));
        }
    }
}
