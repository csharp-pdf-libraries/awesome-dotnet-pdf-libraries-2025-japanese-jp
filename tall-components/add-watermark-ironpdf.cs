```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using IronPdf.Editing;

class Program
{
    static void Main()
    {
        // 既存のPDFを読み込む
        var pdf = PdfDocument.FromFile("input.pdf");
        
        // ウォーターマークを作成
        var watermark = new TextStamper()
        {
            Text = "CONFIDENTIAL",
            FontSize = 60,
            Opacity = 50,
            Rotation = 45,
            VerticalAlignment = VerticalAlignment.Middle,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        
        // すべてのページにウォーターマークを適用
        pdf.ApplyStamp(watermark);
        
        // ウォーターマーク付きPDFを保存
        pdf.SaveAs("watermarked.pdf");
    }
}
```