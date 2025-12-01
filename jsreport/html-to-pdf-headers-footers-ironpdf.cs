```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using IronPdf.Rendering;
using System;

class Program
{
    static void Main(string[] args)
    {
        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.TextHeader = new TextHeaderFooter()
        {
            CenterText = "Custom Header", // カスタムヘッダー
            FontSize = 10
        };
        renderer.RenderingOptions.TextFooter = new TextHeaderFooter()
        {
            CenterText = "Page {page} of {total-pages}", // ページ {page} / {total-pages}
            FontSize = 10
        };
        
        var pdf = renderer.RenderHtmlAsPdf("<h1>Document with Header and Footer</h1><p>Main content goes here.</p>");
        pdf.SaveAs("document_with_headers.pdf");
        Console.WriteLine("PDF with headers and footers created successfully!"); // ヘッダーとフッター付きのPDFが正常に作成されました！
    }
}
```