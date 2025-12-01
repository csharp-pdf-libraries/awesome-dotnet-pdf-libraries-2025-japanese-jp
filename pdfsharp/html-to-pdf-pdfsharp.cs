```csharp
// NuGet: Install-Package PdfSharp
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System;

class Program
{
    static void Main()
    {
        // PDFSharpは組み込みのHTMLからPDFへの変換機能がありません
        // HTMLを手動で解析し、コンテンツをレンダリングする必要があります
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Arial", 12);
        
        // 手動でのテキストレンダリング（HTMLサポートなし）
        gfx.DrawString("Hello from PDFSharp", font, XBrushes.Black,
            new XRect(0, 0, page.Width, page.Height),
            XStringFormats.TopLeft);
        
        document.Save("output.pdf");
    }
}
```