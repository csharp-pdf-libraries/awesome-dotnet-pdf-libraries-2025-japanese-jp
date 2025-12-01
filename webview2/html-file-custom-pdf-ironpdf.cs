```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using IronPdf.Rendering;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape; // 用紙の向きを横に設定
        renderer.RenderingOptions.MarginTop = 50; // 上の余白を設定
        renderer.RenderingOptions.MarginBottom = 50; // 下の余白を設定
        
        string htmlFile = Path.Combine(Directory.GetCurrentDirectory(), "input.html"); // HTMLファイルのパスを結合
        var pdf = renderer.RenderHtmlFileAsPdf(htmlFile); // HTMLファイルからPDFをレンダリング
        pdf.SaveAs("custom.pdf"); // PDFを「custom.pdf」として保存
        
        Console.WriteLine("Custom PDF created"); // カスタムPDFが作成されました
    }
}
```