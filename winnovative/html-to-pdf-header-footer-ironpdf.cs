```csharp
// NuGet: Install-Package IronPdf を使用
using IronPdf;
using IronPdf.Rendering;
using System;

class Program
{
    static void Main()
    {
        // PDF レンダラーを作成
        var renderer = new ChromePdfRenderer();
        
        // ヘッダーとフッターを設定
        renderer.RenderingOptions.TextHeader = new TextHeaderFooter()
        {
            CenterText = "Document Header",
            FontSize = 12
        };
        
        renderer.RenderingOptions.TextFooter = new TextHeaderFooter()
        {
            CenterText = "Page {page} of {total-pages}",
            FontSize = 10
        };
        
        // HTML を PDF に変換
        string htmlString = "<html><body><h1>Document with Header and Footer</h1><p>Content goes here</p></body></html>";
        var pdf = renderer.RenderHtmlAsPdf(htmlString);
        
        // ファイルに保存
        pdf.SaveAs("document.pdf");
        
        Console.WriteLine("PDF with header and footer created successfully");
    }
}
```