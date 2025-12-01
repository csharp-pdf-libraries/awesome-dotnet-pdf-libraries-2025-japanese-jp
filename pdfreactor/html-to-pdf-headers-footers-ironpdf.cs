```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using IronPdf.Rendering;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        renderer.RenderingOptions.TextHeader = new TextHeaderFooter()
        {
            CenterText = "Header Text" // ヘッダーテキスト
        };
        
        renderer.RenderingOptions.TextFooter = new TextHeaderFooter()
        {
            CenterText = "Page {page}" // ページ {page}
        };
        
        string html = "<html><body><h1>Document with Headers</h1><p>Content here</p></body></html>"; // ヘッダー付きドキュメント
        
        var pdf = renderer.RenderHtmlAsPdf(html);
        
        pdf.SaveAs("document.pdf"); // document.pdf として保存
    }
}
```