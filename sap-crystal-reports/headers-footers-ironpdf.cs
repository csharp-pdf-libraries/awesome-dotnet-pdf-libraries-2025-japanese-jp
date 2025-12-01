```csharp
// NuGet: IronPdfをインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        // ヘッダーとフッターを設定する
        renderer.RenderingOptions.TextHeader.CenterText = "Company Name";
        renderer.RenderingOptions.TextHeader.FontSize = 12;
        
        renderer.RenderingOptions.TextFooter.LeftText = "Confidential";
        renderer.RenderingOptions.TextFooter.RightText = "Page {page} of {total-pages}";
        renderer.RenderingOptions.TextFooter.FontSize = 10;
        
        string htmlContent = "<h1>Document Title</h1><p>Document content goes here.</p>";
        
        var pdf = renderer.RenderHtmlAsPdf(htmlContent);
        pdf.SaveAs("output.pdf");
        
        Console.WriteLine("PDF with headers and footers created!");
    }
}
```