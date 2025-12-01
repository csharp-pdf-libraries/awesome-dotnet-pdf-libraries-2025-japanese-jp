```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.TextHeader.CenterText = "Company Header"; // 会社のヘッダー
        renderer.RenderingOptions.TextFooter.CenterText = "Page {page} of {total-pages}"; // ページ {page} / {total-pages}
        renderer.RenderingOptions.MarginTop = 20; // 上余白
        renderer.RenderingOptions.MarginBottom = 20; // 下余白
        var pdf = renderer.RenderUrlAsPdf("https://example.com"); // URLをPDFにレンダリング
        pdf.SaveAs("webpage.pdf"); // "webpage.pdf"として保存
    }
}
```