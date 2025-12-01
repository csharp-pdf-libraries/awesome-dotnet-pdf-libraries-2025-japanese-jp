```csharp
// NuGet: Install-Package IronPdf を使用
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        // PDF レンダラーを作成
        var renderer = new ChromePdfRenderer();
        
        // ページサイズと向きを設定
        renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;
        
        // URL を PDF に変換
        var pdf = renderer.RenderUrlAsPdf("https://www.example.com");
        
        // ファイルに保存
        pdf.SaveAs("webpage.pdf");
        
        Console.WriteLine("PDF from URL created successfully!");
    }
}
```