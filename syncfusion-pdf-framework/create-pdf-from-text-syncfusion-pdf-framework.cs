```csharp
// NuGet: Syncfusion.Pdf.Net.Core をインストール
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using System.IO;

class Program
{
    static void Main()
    {
        // 新しいPDFドキュメントを作成
        PdfDocument document = new PdfDocument();
        
        // ページを追加
        PdfPage page = document.Pages.Add();
        
        // フォントを作成
        PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
        
        // テキストを描画
        page.Graphics.DrawString("Hello, World!", font, PdfBrushes.Black, new PointF(10, 10));
        
        // ドキュメントを保存
        FileStream fileStream = new FileStream("Output.pdf", FileMode.Create);
        document.Save(fileStream);
        document.Close(true);
        fileStream.Close();
    }
}
```