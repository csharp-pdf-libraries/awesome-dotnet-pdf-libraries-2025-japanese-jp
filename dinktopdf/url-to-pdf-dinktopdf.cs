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
                Orientation = Orientation.Portrait, // 用紙の向きを縦に設定
                PaperSize = PaperKind.A4, // 用紙サイズをA4に設定
            },
            Objects = {
                new ObjectSettings() {
                    Page = "https://www.example.com", // PDFに変換するWebページのURL
                }
            }
        };
        byte[] pdf = converter.Convert(doc); // HTMLをPDFに変換
        File.WriteAllBytes("webpage.pdf", pdf); // PDFファイルを保存
    }
}
```