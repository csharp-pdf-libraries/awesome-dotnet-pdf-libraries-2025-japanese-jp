```csharp
// NuGet: Install-Package Api2Pdf.DotNet
using System;
using System.IO;
using System.Threading.Tasks;
using Api2Pdf.DotNet;

class Program
{
    static async Task Main(string[] args)
    {
        var a2pClient = new Api2PdfClient("your-api-key");
        string html = File.ReadAllText("input.html");
        var options = new HeadlessChromeOptions
        {
            Landscape = true, // 横向き
            PrintBackground = true, // 背景を印刷する
        };
        var apiResponse = await a2pClient.HeadlessChrome.FromHtmlAsync(html, options);
        Console.WriteLine(apiResponse.Pdf);
    }
}
```