```csharp
// NuGet: Install-Package PuppeteerSharp
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true // ヘッドレスモードで起動
        });
        
        await using var page = await browser.NewPageAsync(); // 新しいページを開く
        await page.SetContentAsync("<h1>Custom PDF</h1><p>With landscape orientation and margins.</p>"); // コンテンツを設定
        
        await page.PdfAsync("custom.pdf", new PdfOptions
        {
            Format = PaperFormat.A4, // A4サイズ
            Landscape = true, // 横向き
            MarginOptions = new MarginOptions // 余白設定
            {
                Top = "20mm",
                Bottom = "20mm",
                Left = "20mm",
                Right = "20mm"
            }
        });
    }
}
```