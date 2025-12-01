```csharp
// NuGet: Install-Package WkHtmlToPdf-DotNet をインストール
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
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
                Margins = new MarginSettings() { Top = 10, Bottom = 10, Left = 10, Right = 10 } // 余白を設定
            },
            Objects = {
                new ObjectSettings()
                {
                    Page = "input.html", // 変換するHTMLページ
                    WebSettings = { DefaultEncoding = "utf-8" } // ウェブの設定：デフォルトエンコーディングをutf-8に設定
                }
            }
        };
        byte[] pdf = converter.Convert(doc); // PDFに変換
        File.WriteAllBytes("custom-output.pdf", pdf); // PDFをファイルに保存
    }
}
```