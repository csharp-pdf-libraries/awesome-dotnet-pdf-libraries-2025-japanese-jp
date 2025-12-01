```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        // PDFドキュメントを読み込む
        var pdf1 = PdfDocument.FromFile("Document1.pdf");
        var pdf2 = PdfDocument.FromFile("Document2.pdf");
        
        // PDFをマージ
        var merged = PdfDocument.Merge(new List<PdfDocument> { pdf1, pdf2 });
        
        // マージしたドキュメントを保存
        merged.SaveAs("Merged.pdf");
    }
}
```