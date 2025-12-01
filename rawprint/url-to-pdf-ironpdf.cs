```csharp
// NuGet: IronPdfをインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        // CSS、JavaScript、画像を完全にサポートして、ライブウェブサイトを直接PDFにレンダリングする
        var pdf = renderer.RenderUrlAsPdf("https://example.com");
        pdf.SaveAs("webpage.pdf");
        Console.WriteLine("Website rendered to PDF successfully");
    }
}
```