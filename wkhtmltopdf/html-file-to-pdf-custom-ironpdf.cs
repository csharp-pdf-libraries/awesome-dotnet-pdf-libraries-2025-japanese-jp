```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using IronPdf.Rendering;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape; // 用紙の向きを横向きに設定
        renderer.RenderingOptions.MarginTop = 10; // 上マージンを10に設定
        renderer.RenderingOptions.MarginBottom = 10; // 下マージンを10に設定
        renderer.RenderingOptions.MarginLeft = 10; // 左マージンを10に設定
        renderer.RenderingOptions.MarginRight = 10; // 右マージンを10に設定
        renderer.RenderingOptions.PaperSize = PdfPaperSize.A4; // 用紙サイズをA4に設定
        
        var pdf = renderer.RenderHtmlFileAsPdf("input.html"); // HTMLファイルをPDFに変換
        pdf.SaveAs("custom-output.pdf"); // 変換したPDFを"custom-output.pdf"として保存
    }
}
```