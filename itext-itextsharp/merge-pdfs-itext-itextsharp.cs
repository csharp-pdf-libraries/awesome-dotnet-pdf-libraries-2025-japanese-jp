```csharp
// NuGet: Install-Package itext7
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System.IO;

class Program
{
    static void Main()
    {
        string outputPath = "merged.pdf"; // 出力パスを設定
        string[] inputFiles = { "document1.pdf", "document2.pdf", "document3.pdf" }; // 入力ファイルの配列
        
        using (PdfWriter writer = new PdfWriter(outputPath)) // 出力PDFのためのライター
        using (PdfDocument pdfDoc = new PdfDocument(writer)) // PDFドキュメントを作成
        {
            PdfMerger merger = new PdfMerger(pdfDoc); // PDFマージャーのインスタンスを作成
            
            foreach (string file in inputFiles) // 入力ファイルをループ処理
            {
                using (PdfDocument sourcePdf = new PdfDocument(new PdfReader(file))) // ソースPDFを開く
                {
                    merger.Merge(sourcePdf, 1, sourcePdf.GetNumberOfPages()); // ページをマージ
                }
            }
        }
    }
}
```