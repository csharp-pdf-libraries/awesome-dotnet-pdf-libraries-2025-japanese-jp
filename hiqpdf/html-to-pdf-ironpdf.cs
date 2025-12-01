```csharp
// NuGet: IronPdfをインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderUrlAsPdf("https://example.com");
        pdf.SaveAs("output.pdf");
        
        // HTML文字列を変換
        string html = "<h1>Hello World</h1><p>This is a PDF document.</p>";
        var pdfFromHtml = renderer.RenderHtmlAsPdf(html);
        pdfFromHtml.SaveAs("fromhtml.pdf");
    }
}
```