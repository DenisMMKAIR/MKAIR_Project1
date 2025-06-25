using Infrastructure.DocumentProcessor;
using Infrastructure.DocumentProcessor.Creator;

namespace Project1Tests.DocumentProcessor;

[TestFixture]
public abstract class CombinedFixture
{
    protected ManometrDocumentCreator ManometrCreator { get; set; }
    protected PDFExporter PdfExporter { get; set; }

    [OneTimeSetUp]
    public void Setup()
    {
        ManometrCreator = new ManometrDocumentCreator(Tools.GetSignsDirPath());
        PdfExporter = new PDFExporter();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await PdfExporter.DisposeAsync();
    }
}
