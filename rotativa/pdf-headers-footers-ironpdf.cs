```csharp
// NuGet: Install-Package IronPdf を使用
using IronPdf;
using IronPdf.Rendering;
using System;

namespace IronPdfExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var renderer = new ChromePdfRenderer();
            
            renderer.RenderingOptions.TextHeader = new TextHeaderFooter()
            {
                CenterText = "Page Header", // ページヘッダー
                DrawDividerLine = true // 区切り線を描画する
            };
            
            renderer.RenderingOptions.TextFooter = new TextHeaderFooter()
            {
                CenterText = "Page {page} of {total-pages}", // ページ {page} / {total-pages}
                DrawDividerLine = true // 区切り線を描画する
            };
            
            var htmlContent = "<h1>Report Title</h1><p>Report content goes here.</p>"; // レポートのタイトルと内容
            var pdf = renderer.RenderHtmlAsPdf(htmlContent);
            pdf.SaveAs("report.pdf"); // PDFを保存
            
            Console.WriteLine("PDF with headers and footers created successfully!"); // ヘッダーとフッター付きのPDFが正常に作成されました！
        }
    }
}
```