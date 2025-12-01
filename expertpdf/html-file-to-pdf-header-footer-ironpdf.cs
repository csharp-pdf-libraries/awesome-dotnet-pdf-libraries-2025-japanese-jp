```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        // PDFレンダラーを作成
        var renderer = new ChromePdfRenderer();
        
        // ヘッダーを設定
        renderer.RenderingOptions.TextHeader = new TextHeaderFooter()
        {
            CenterText = "Document Header",
            DrawDividerLine = true
        };
        
        // ページ番号付きのフッターを設定
        renderer.RenderingOptions.TextFooter = new TextHeaderFooter()
        {
            RightText = "Page {page} of {total-pages}",
            DrawDividerLine = true
        };
        
        // HTMLファイルをPDFに変換
        var pdf = renderer.RenderHtmlFileAsPdf("input.html");
        
        // ファイルに保存
        pdf.SaveAs("output-with-header-footer.pdf");
        
        Console.WriteLine("PDF with headers and footers created successfully!");
    }
}
```