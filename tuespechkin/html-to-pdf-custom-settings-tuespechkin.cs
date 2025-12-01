```csharp
// NuGet: Install-Package TuesPechkin をインストール
using TuesPechkin;
using System.IO;

class Program
{
    static void Main()
    {
        var converter = new StandardConverter(
            new RemotingToolset<PdfToolset>(
                new Win64EmbeddedDeployment(
                    new TempFolderDeployment())));
        
        string html = "<html><body><h1>Custom PDF</h1></body></html>";
        
        var document = new HtmlToPdfDocument
        {
            GlobalSettings = {
                Orientation = GlobalSettings.PdfOrientation.Landscape, // オリエンテーションを横向きに設定
                PaperSize = GlobalSettings.PdfPaperSize.A4, // 用紙サイズをA4に設定
                Margins = new MarginSettings { Unit = Unit.Millimeters, Top = 10, Bottom = 10 } // 上下の余白を10mmに設定
            },
            Objects = {
                new ObjectSettings { HtmlText = html }
            }
        };
        
        byte[] pdfBytes = converter.Convert(document); // HTMLをPDFに変換
        File.WriteAllBytes("custom.pdf", pdfBytes); // PDFをファイルに保存
    }
}
```