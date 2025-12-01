```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System;

class IronPdfHtmlToPdf
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        string htmlContent = "<html><body><h1>Hello World</h1></body></html>";
        
        // 簡単なAPIを使ってHTMLをPDFに変換
        var pdf = renderer.RenderHtmlAsPdf(htmlContent);
        pdf.SaveAs("output.pdf");
    }
}
```