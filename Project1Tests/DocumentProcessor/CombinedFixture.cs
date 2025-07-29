using Infrastructure.DocumentProcessor;
using Infrastructure.DocumentProcessor.Creator;
using Microsoft.Extensions.Logging;

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
        var logger = new LoggerFactory().CreateLogger<PDFExporter>();
        PdfExporter = new PDFExporter(logger);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await PdfExporter.DisposeAsync();
    }
}
