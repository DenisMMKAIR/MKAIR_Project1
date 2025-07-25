using Infrastructure.DocumentProcessor;
using Infrastructure.DocumentProcessor.Creator;

namespace Project1Tests.DocumentProcessor;

[TestFixture]
internal abstract class CombinedFixture
{
    protected ManometrSuccessDocumentCreator ManometrCreator { get; set; }
    protected DavlenieSuccessDocumentCreator DavlenieCreator { get; set; }
    protected PDFExporter PdfExporter { get; set; }

    [OneTimeSetUp]
    public void Setup()
    {
        ManometrCreator = new ManometrSuccessDocumentCreator([], Tools.GetSignsDirPath());
        DavlenieCreator = new DavlenieSuccessDocumentCreator([], Tools.GetSignsDirPath());
        PdfExporter = new PDFExporter();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await PdfExporter.DisposeAsync();
    }
}
