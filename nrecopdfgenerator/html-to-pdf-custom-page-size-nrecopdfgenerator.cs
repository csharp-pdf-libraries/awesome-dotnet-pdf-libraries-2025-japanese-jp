```csharp
// NuGet: Install-Package NReco.PdfGenerator をインストール
using NReco.PdfGenerator;
using System.IO;

class Program
{
    static void Main()
    {
        var htmlToPdf = new HtmlToPdfConverter();
        htmlToPdf.PageWidth = 210; // ページ幅を設定
        htmlToPdf.PageHeight = 297; // ページ高さを設定
        htmlToPdf.Margins = new PageMargins { Top = 10, Bottom = 10, Left = 10, Right = 10 }; // マージンを設定
        var htmlContent = "<html><body><h1>Custom Page Size</h1><p>A4 size document with margins.</p></body></html>"; // HTMLコンテンツ
        var pdfBytes = htmlToPdf.GeneratePdf(htmlContent); // PDFを生成
        File.WriteAllBytes("custom-size.pdf", pdfBytes); // PDFをファイルに書き出す
    }
}
```