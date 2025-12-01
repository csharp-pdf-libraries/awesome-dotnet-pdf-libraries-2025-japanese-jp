```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using IronPdf.Rendering;
using System;

class IronPdfUrlToPdf
{
    static void Main()
    {
        // ChromePdfRenderer インスタンスを作成
        var renderer = new ChromePdfRenderer();
        
        // ヘッダーとフッターを含むレンダリングオプションを設定
        renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter()
        {
            HtmlFragment = "<div style='text-align:center'>Company Report</div>"
        };
        
        renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter()
        {
            HtmlFragment = "<div style='text-align:center'>Page {page} of {total-pages} - " + DateTime.Now.ToString("MM/dd/yyyy") + "</div>"
        };
        
        // URL を PDF に変換
        string url = "https://example.com";
        var pdf = renderer.RenderUrlAsPdf(url);
        
        // PDF ファイルを保存
        pdf.SaveAs("webpage.pdf");
    }
}
```