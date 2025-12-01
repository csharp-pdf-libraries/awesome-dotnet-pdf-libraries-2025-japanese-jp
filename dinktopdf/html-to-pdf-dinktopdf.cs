```csharp
// NuGet: Install-Package DinkToPdf をインストール
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
                Orientation = Orientation.Portrait, // オリエンテーションを縦向きに設定
                PaperSize = PaperKind.A4, // 用紙サイズをA4に設定
            },
            Objects = {
                new ObjectSettings() {
                    HtmlContent = "<h1>Hello World</h1><p>This is a PDF from HTML.</p>", // HTMLコンテンツを設定
                    WebSettings = { DefaultEncoding = "utf-8" } // Web設定でデフォルトエンコーディングをutf-8に設定
                }
            }
        };
        byte[] pdf = converter.Convert(doc); // PDFに変換
        File.WriteAllBytes("output.pdf", pdf); // ファイルに書き込み
    }
}
```