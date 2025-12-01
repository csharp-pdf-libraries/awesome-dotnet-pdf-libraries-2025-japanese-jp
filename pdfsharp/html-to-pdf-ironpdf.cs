```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        // IronPDFはネイティブHTMLからPDFへのレンダリングを持っています
        var renderer = new ChromePdfRenderer();
        
        string html = "<h1>Hello from IronPDF</h1><p>Easy HTML to PDF conversion</p>";
        var pdf = renderer.RenderHtmlAsPdf(html);
        
        pdf.SaveAs("output.pdf");
    }
}
```