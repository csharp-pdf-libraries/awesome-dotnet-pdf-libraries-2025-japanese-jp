```csharp
// NuGet: Install-Package WkHtmlToPdf-DotNet
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
                ColorMode = ColorMode.Color, // 色モードをカラーに設定
                Orientation = Orientation.Portrait, // 用紙の向きを縦に設定
                PaperSize = PaperKind.A4 // 用紙サイズをA4に設定
            },
            Objects = {
                new ObjectSettings()
                {
                    Page = "https://www.example.com" // PDFに変換するWebページのURL
                }
            }
        };
        byte[] pdf = converter.Convert(doc); // HTMLをPDFに変換
        File.WriteAllBytes("webpage.pdf", pdf); // PDFファイルを保存
    }
}
```