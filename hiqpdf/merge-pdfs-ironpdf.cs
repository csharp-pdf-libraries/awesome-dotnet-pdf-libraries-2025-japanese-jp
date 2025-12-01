```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        // 最初のPDFを作成
        var pdf1 = renderer.RenderHtmlAsPdf("<h1>First Document</h1>");
        pdf1.SaveAs("doc1.pdf");
        
        // 二番目のPDFを作成
        var pdf2 = renderer.RenderHtmlAsPdf("<h1>Second Document</h1>");
        pdf2.SaveAs("doc2.pdf");
        
        // PDFを結合
        var merged = PdfDocument.Merge(pdf1, pdf2);
        merged.SaveAs("merged.pdf");
    }
}
```