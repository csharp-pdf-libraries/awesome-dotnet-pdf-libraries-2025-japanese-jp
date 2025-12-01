```csharp
// NuGet: Install-Package PdfiumViewer
using PdfiumViewer;
using System;
using System.Drawing;
using System.Drawing.Imaging;

string pdfPath = "document.pdf";
string outputImage = "page1.png";

// PDFiumViewerはPDFを画像にレンダリングするのに優れています
using (var document = PdfDocument.Load(pdfPath))
{
    // 300 DPIで最初のページをレンダリング
    int dpi = 300;
    using (var image = document.Render(0, dpi, dpi, true))
    {
        // PNGとして保存
        image.Save(outputImage, ImageFormat.Png);
        Console.WriteLine($"Page rendered to {outputImage}");
    }
    
    // 全ページをレンダリング
    for (int i = 0; i < document.PageCount; i++)
    {
        using (var pageImage = document.Render(i, 150, 150, true))
        {
            pageImage.Save($"page_{i + 1}.png", ImageFormat.Png);
        }
    }
}
```