```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        // ヘッダーとフッターを設定する
        renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter()
        {
            HtmlFragment = "<div style='text-align:center'>Document Header</div>"
        };
        
        renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter()
        {
            HtmlFragment = "<div style='text-align:center'>Page {page} of {total-pages}</div>"
        };
        
        var pdf = renderer.RenderHtmlAsPdf("<html><body><h1>Report Content</h1><p>This is the main content.</p></body></html>");
        pdf.SaveAs("report.pdf");
    }
}
```