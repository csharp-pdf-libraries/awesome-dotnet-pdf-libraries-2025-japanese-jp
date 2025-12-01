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
                ColorMode = ColorMode.Color, // カラーモードをカラーに設定
                Orientation = Orientation.Portrait, // オリエンテーションをポートレートに設定
                PaperSize = PaperKind.A4 // 用紙サイズをA4に設定
            },
            Objects = {
                new ObjectSettings()
                {
                    HtmlContent = "<h1>Hello World</h1><p>This is a PDF from HTML.</p>" // HTMLコンテンツを設定
                }
            }
        };
        byte[] pdf = converter.Convert(doc); // PDFに変換
        File.WriteAllBytes("output.pdf", pdf); // PDFをファイルに保存
    }
}
```