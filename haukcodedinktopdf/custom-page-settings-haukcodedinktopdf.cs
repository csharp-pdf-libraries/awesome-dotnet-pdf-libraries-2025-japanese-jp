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
                Orientation = Orientation.Landscape, // オリエンテーションをランドスケープに設定
                PaperSize = PaperKind.Letter, // 用紙サイズをレターに設定
                Margins = new MarginSettings() { Top = 10, Bottom = 10, Left = 10, Right = 10 } // 余白設定
            },
            Objects = {
                new ObjectSettings() {
                    HtmlContent = "<html><body><h1>Landscape Document</h1><p>Custom page settings</p></body></html>", // HTMLコンテンツ
                }
            }
        };
        
        byte[] pdf = converter.Convert(doc); // PDFに変換
        File.WriteAllBytes("landscape.pdf", pdf); // ファイルに書き出し
    }
}
```