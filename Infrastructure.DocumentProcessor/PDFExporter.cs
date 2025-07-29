using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace Infrastructure.DocumentProcessor;

internal class PDFExporter : IAsyncDisposable
{
    private readonly ILogger _logger;
    private readonly Task _setup;
    private IBrowser _browser = null!;

    public PDFExporter(ILogger logger)
    {
        _logger = logger;
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

    public async Task<bool> ExportAsync(string htmlContent, string outputFilePath, CancellationToken cancellationToken = default)
    {
        try
        {
            await _setup;
            using var page = await _browser.NewPageAsync();

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            await page.SetContentAsync(htmlContent);
            await page.PdfAsync(outputFilePath);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error exporting PDF");
            return false;
        }
    }
}
