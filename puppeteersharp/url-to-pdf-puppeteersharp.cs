```csharp
// NuGet: Install-Package PuppeteerSharp をインストール
using PuppeteerSharp;
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
        await page.GoToAsync("https://www.example.com"); // 指定のURLにアクセス
        await page.PdfAsync("webpage.pdf"); // ページをPDFとして保存
    }
}
```