```csharp
// NuGet: Install-Package PdfSharp
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using System;

class Program
{
    static void Main()
    {
        // 既存のPDFを開く
        PdfDocument document = PdfReader.Open("existing.pdf", PdfDocumentOpenMode.Modify);
        PdfPage page = document.Pages[0];
        
        // グラフィックスオブジェクトを取得
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Arial", 20, XFontStyle.Bold);
        
        // 特定の位置にテキストを描画
        gfx.DrawString("Watermark Text", font, XBrushes.Red,
            new XPoint(200, 400));
        
        document.Save("modified.pdf");
    }
}
```