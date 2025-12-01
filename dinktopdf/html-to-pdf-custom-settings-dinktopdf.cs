```csharp
// NuGet: Install-Package DinkToPdf
using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;

class Program
{
    static void Main()
    {
        var converter = new SynchronizedConverter(new PdfTools());
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                ColorMode = ColorMode.Color, // カラーモードをカラーに設定
                Orientation = Orientation.Landscape, // 用紙の向きを横に設定
                PaperSize = PaperKind.A4, // 用紙サイズをA4に設定
                Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 15, Right = 15 } // 上下左右の余白を設定
            },
            Objects = {
                new ObjectSettings() {
                    HtmlContent = "<h1>Custom PDF</h1><p>Landscape orientation with custom margins.</p>", // HTMLコンテンツ
                    WebSettings = { DefaultEncoding = "utf-8" } // Web設定: デフォルトエンコーディングをutf-8に設定
                }
            }
        };
        byte[] pdf = converter.Convert(doc); // PDFに変換
        File.WriteAllBytes("custom.pdf", pdf); // ファイルに書き出し
    }
}
```