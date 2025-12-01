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
                Orientation = Orientation.Portrait, // 用紙の向きを縦に設定
                PaperSize = PaperKind.A4, // 用紙サイズをA4に設定
            },
            Objects = {
                new ObjectSettings() {
                    HtmlContent = "<html><body><h1>Hello World</h1></body></html>", // HTMLコンテンツを設定
                }
            }
        };
        
        byte[] pdf = converter.Convert(doc); // PDFに変換
        File.WriteAllBytes("output.pdf", pdf); // ファイルに書き出し
    }
}
```