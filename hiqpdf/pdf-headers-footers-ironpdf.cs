```csharp
// NuGet: Install-Package IronPdf を使用
using IronPdf;
using IronPdf.Rendering;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        // ヘッダーとフッターを設定
        renderer.RenderingOptions.TextHeader = new TextHeaderFooter()
        {
            CenterText = "Page Header",
            FontSize = 12
        };
        
        renderer.RenderingOptions.TextFooter = new TextHeaderFooter()
        {
            CenterText = "Page {page} of {total-pages}",
            FontSize = 10
        };
        
        var pdf = renderer.RenderHtmlAsPdf("<h1>Document with Headers and Footers</h1>");
        pdf.SaveAs("header-footer.pdf");
    }
}
```