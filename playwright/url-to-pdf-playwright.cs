```csharp
// NuGet: Install-Package Microsoft.Playwright をインストール
using Microsoft.Playwright;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var playwright = await Playwright.CreateAsync(); // Playwrightを作成
        var browser = await playwright.Chromium.LaunchAsync(); // Chromiumブラウザを起動
        var page = await browser.NewPageAsync(); // 新しいページを開く
        
        await page.GotoAsync("https://www.example.com"); // 指定したURLに移動
        await page.PdfAsync(new PagePdfOptions 
        { 
            Path = "webpage.pdf", // PDFの保存パス
            Format = "A4" // PDFのフォーマット
        });
        
        await browser.CloseAsync(); // ブラウザを閉じる
    }
}
```