```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using IronPdf.Engines.Chrome;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape; // 用紙の向きを横向きに設定
        renderer.RenderingOptions.PaperSize = PdfPaperSize.A4; // 用紙サイズをA4に設定
        renderer.RenderingOptions.MarginTop = 10; // 上マージンを10に設定
        renderer.RenderingOptions.MarginBottom = 10; // 下マージンを10に設定
        
        string html = "<html><body><h1>Custom PDF</h1></body></html>"; // HTMLコンテンツ
        var pdf = renderer.RenderHtmlAsPdf(html); // HTMLからPDFを生成
        
        pdf.SaveAs("custom.pdf"); // "custom.pdf"として保存
    }
}
```