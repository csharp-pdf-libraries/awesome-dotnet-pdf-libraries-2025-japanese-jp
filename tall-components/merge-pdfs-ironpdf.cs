```csharp
// NuGet: IronPdfをインストール
using IronPdf;

class Program
{
    static void Main()
    {
        // PDFを読み込む
        var pdf1 = PdfDocument.FromFile("document1.pdf");
        var pdf2 = PdfDocument.FromFile("document2.pdf");
        
        // PDFを統合
        var merged = PdfDocument.Merge(pdf1, pdf2);
        
        // 統合されたドキュメントを保存
        merged.SaveAs("merged.pdf");
    }
}
```