```csharp
// NuGet: Install-Package IronPdf を使用
using IronPdf;
using System;

class IronPdfMergePdfs
{
    static void Main()
    {
        // PDFドキュメントを読み込む
        var pdf1 = PdfDocument.FromFile("document1.pdf");
        var pdf2 = PdfDocument.FromFile("document2.pdf");
        
        // 簡単な方法でPDFをマージ
        var merged = PdfDocument.Merge(pdf1, pdf2);
        merged.SaveAs("merged.pdf");
    }
}
```