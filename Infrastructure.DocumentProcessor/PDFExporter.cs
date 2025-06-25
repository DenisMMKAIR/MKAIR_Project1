using PuppeteerSharp;

namespace Infrastructure.DocumentProcessor;

public class PDFExporter : IAsyncDisposable
{
    private readonly Task _setup;
    private IBrowser _browser = null!;

    public PDFExporter()
    {
        var t1 = new BrowserFetcher().DownloadAsync();
        var t2 = t1.ContinueWith(_ => Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }));
        _setup = t2.ContinueWith(_ => _browser = _.Result.Result);
    }

    public async ValueTask DisposeAsync()
    {
        await _setup;
        if (_browser == null) return;
        await _browser.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public async Task<bool> ExportAsync(string htmlContent, string outputFilePath)
    {
        await _setup;
        using var page = await _browser.NewPageAsync();
        await page.SetContentAsync(htmlContent);
        await page.PdfAsync(outputFilePath);
        return true;
    }
}
