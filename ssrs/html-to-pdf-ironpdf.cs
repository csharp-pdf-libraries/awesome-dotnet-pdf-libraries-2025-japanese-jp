```csharp
// NuGet: IronPdfをインストール
using IronPdf;
using System;

class IronPdfHtmlToPdf
{
    static void Main()
    {
        // ChromePdfRenderer インスタンスを作成
        var renderer = new ChromePdfRenderer();
        
        // HTML文字列をPDFに変換
        var htmlContent = "<h1>Hello World</h1><p>This is HTML content.</p>";
        var pdf = renderer.RenderHtmlAsPdf(htmlContent);
        
        // PDFファイルを保存
        pdf.SaveAs("output.pdf");
    }
}
```