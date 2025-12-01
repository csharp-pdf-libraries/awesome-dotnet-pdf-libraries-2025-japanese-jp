```csharp
// NuGet: Install-Package Syncfusion.Pdf.Net.Core
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System.IO;

class Program
{
    static void Main()
    {
        // 最初のPDFドキュメントをロード
        FileStream stream1 = new FileStream("Document1.pdf", FileMode.Open, FileAccess.Read);
        PdfLoadedDocument loadedDocument1 = new PdfLoadedDocument(stream1);
        
        // 2番目のPDFドキュメントをロード
        FileStream stream2 = new FileStream("Document2.pdf", FileMode.Open, FileAccess.Read);
        PdfLoadedDocument loadedDocument2 = new PdfLoadedDocument(stream2);
        
        // ドキュメントをマージ
        PdfDocument finalDocument = new PdfDocument();
        finalDocument.ImportPageRange(loadedDocument1, 0, loadedDocument1.Pages.Count - 1);
        finalDocument.ImportPageRange(loadedDocument2, 0, loadedDocument2.Pages.Count - 1);
        
        // マージされたドキュメントを保存
        FileStream outputStream = new FileStream("Merged.pdf", FileMode.Create);
        finalDocument.Save(outputStream);
        
        // すべてのドキュメントを閉じる
        finalDocument.Close(true);
        loadedDocument1.Close(true);
        loadedDocument2.Close(true);
        stream1.Close();
        stream2.Close();
        outputStream.Close();
    }
}
```