```csharp
// NuGet: Install-Package IronPdf をインストールします
using System;
using IronPdf;
using IronPdf.Rendering;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.TextHeader = new TextHeaderFooter()
        {
            CenterText = "ページヘッダー"
        };
        renderer.RenderingOptions.TextFooter = new TextHeaderFooter()
        {
            CenterText = "ページ {page} / {total-pages}"
        };
        
        var pdf = renderer.RenderHtmlAsPdf("<h1>ドキュメントの内容</h1>");
        pdf.SaveAs("document.pdf");
    }
}
```