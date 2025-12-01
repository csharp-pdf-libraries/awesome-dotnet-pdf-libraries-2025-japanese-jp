```csharp
// NuGet: IronPdfをインストールする
using IronPdf;
using IronPdf.Editing;
using System;

class IronPdfAddWatermark
{
    static void Main()
    {
        var pdf = PdfDocument.FromFile("input.pdf");
        
        // シンプルなAPIを使用してテキストウォーターマークを適用
        pdf.ApplyWatermark("<h1 style='color:red; opacity:0.5;'>CONFIDENTIAL</h1>",
            rotation: 45,
            verticalAlignment: VerticalAlignment.Middle,
            horizontalAlignment: HorizontalAlignment.Center);
        
        pdf.SaveAs("watermarked.pdf");
    }
}
```