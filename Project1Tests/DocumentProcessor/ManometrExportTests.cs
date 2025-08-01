using ProjApp.ProtocolForms;

namespace Project1Tests.DocumentProcessor;

internal class ManometrExportTests : CombinedFixture
{
    [Test]
    public async Task Manometr_DeviceInfo_Short()
    {
        await PDFTest(
            DummyManometr1Data.CreateForm(deviceInfo: "Манометры избыточного давления; 232.30.160"),
            "ManometrDeviceInfoShort.pdf");
    }

    [Test]
    public async Task Manometr_DeviceInfo_Long()
    {
        await PDFTest(
            DummyManometr1Data.CreateForm(),
            "ManometrDeviceInfoLong.pdf");
    }

    [Test]
    public async Task Manometr_VerificationMethod_Short()
    {
        await PDFTest(
            DummyManometr1Data.CreateForm(vmCount: 1),
            "ManometrVerificationMethodShort.pdf");
    }

    [Test]
    public async Task Manometr_VerificationMethod_Medium()
    {
        await PDFTest(
            DummyManometr1Data.CreateForm(vmCount: 2),
            "ManometrVerificationMethodMedium.pdf");
    }

    [Test]
    public async Task Manometr_VerificationMethod_Long()
    {
        await PDFTest(
            DummyManometr1Data.CreateForm(vmCount: 4),
            "ManometrVerificationMethodLong.pdf");
    }

    [Test]
    public async Task Manometr_Etalons_1()
    {
        await PDFTest(
            DummyManometr1Data.CreateForm(etaCount: 1),
            "ManometrEtalons1.pdf");
    }

    [Test]
    public async Task Manometr_Etalons_2()
    {
        await PDFTest(
            DummyManometr1Data.CreateForm(etaCount: 2),
            "ManometrEtalons2.pdf");
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