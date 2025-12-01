```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using IronPdf.Editing;
using System;

class Program
{
    static void Main()
    {
        // 既存のPDFを開く
        var pdf = PdfDocument.FromFile("existing.pdf");
        
        // テキストスタンプ/ウォーターマークを追加
        var textStamper = new TextStamper()
        {
            Text = "Watermark Text",
            FontSize = 20,
            Color = IronSoftware.Drawing.Color.Red,
            VerticalAlignment = VerticalAlignment.Middle,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        
        pdf.ApplyStamp(textStamper);
        pdf.SaveAs("modified.pdf");
    }
}
```