```csharp
// NuGet: Install-Package Microsoft.Web.WebView2.WinForms
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

class Program
{
    static async Task Main()
    {
        var webView = new WebView2();
        await webView.EnsureCoreWebView2Async();
        
        var htmlPath = Path.GetFullPath("document.html");
        var tcs = new TaskCompletionSource<bool>();
        webView.CoreWebView2.NavigationCompleted += (s, e) => tcs.SetResult(true);
        
        webView.CoreWebView2.Navigate($"file:///{htmlPath}");
        await tcs.Task;
        await Task.Delay(1000);
        
        var options = new
        {
            landscape = false, // 横向き = false
            printBackground = true, // 背景を印刷 = true
            paperWidth = 8.5, // 用紙の幅 = 8.5
            paperHeight = 11, // 用紙の高さ = 11
            marginTop = 0.4, // 上の余白 = 0.4
            marginBottom = 0.4, // 下の余白 = 0.4
            marginLeft = 0.4, // 左の余白 = 0.4
            marginRight = 0.4 // 右の余白 = 0.4
        };
        
        var result = await webView.CoreWebView2.CallDevToolsProtocolMethodAsync(
            "Page.printToPDF",
            JsonSerializer.Serialize(options)
        );
        
        var base64 = JsonDocument.Parse(result).RootElement.GetProperty("data").GetString();
        File.WriteAllBytes("output.pdf", Convert.FromBase64String(base64));
    }
}
```