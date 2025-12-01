```csharp
// NuGet: Install-Package IronPdf
using System;
using System.IO;
using IronPdf;

class Program
{
    static void Main(string[] args)
    {
        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Landscape; // 用紙の向きを横向きに設定
        renderer.RenderingOptions.CssMediaType = IronPdf.Rendering.PdfCssMediaType.Print; // CSSメディアタイプを印刷に設定
        string html = File.ReadAllText("input.html"); // HTMLファイルを読み込む
        var pdf = renderer.RenderHtmlAsPdf(html); // HTMLをPDFに変換
        pdf.SaveAs("output.pdf"); // PDFをファイルに保存
        Console.WriteLine("PDF created with options successfully"); // オプションを使用してPDFが正常に作成されました
    }
}
```