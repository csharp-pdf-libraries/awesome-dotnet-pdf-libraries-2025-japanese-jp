```csharp
// NuGet: Install-Package PdfSharp
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System;

class Program
{
    static void Main()
    {
        // 新しいPDFドキュメントを作成
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        
        // 画像を読み込んで描画
        XImage image = XImage.FromFile("image.jpg");
        
        // ページに合わせてサイズを計算
        double width = 200;
        double height = 200;
        
        gfx.DrawImage(image, 50, 50, width, height);
        
        // テキストを追加
        XFont font = new XFont("Arial", 16);
        gfx.DrawString("Image in PDF", font, XBrushes.Black,
            new XPoint(50, 270));
        
        document.Save("output.pdf");
    }
}
```