```csharp
// NuGet: Install-Package IronPdf をインストール
using System;
using IronPdf;
using IronPdf.Rendering;

class IronPdfCustomSize
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter; // レターサイズの紙に設定
        renderer.RenderingOptions.MarginTop = 50; // 上マージンを50に設定
        renderer.RenderingOptions.MarginBottom = 50; // 下マージンを50に設定
        
        var html = "<html><body><h1>Custom Size PDF</h1></body></html>"; // HTMLコンテンツ
        var pdf = renderer.RenderHtmlAsPdf(html); // HTMLからPDFを生成
        
        pdf.SaveAs("custom-size.pdf"); // "custom-size.pdf"として保存
        Console.WriteLine("Custom size PDF generated successfully"); // カスタムサイズのPDFが正常に生成されました
    }
}
```