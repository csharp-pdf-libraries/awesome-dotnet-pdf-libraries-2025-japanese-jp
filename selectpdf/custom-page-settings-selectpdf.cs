```csharp
// NuGet: Install-Package Select.HtmlToPdf をインストール
using SelectPdf;
using System;

class Program
{
    static void Main()
    {
        HtmlToPdf converter = new HtmlToPdf();
        
        converter.Options.PdfPageSize = PdfPageSize.A4; // PDFページサイズをA4に設定
        converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait; // PDFページの向きを縦に設定
        converter.Options.MarginTop = 20; // 上マージンを20に設定
        converter.Options.MarginBottom = 20; // 下マージンを20に設定
        converter.Options.MarginLeft = 20; // 左マージンを20に設定
        converter.Options.MarginRight = 20; // 右マージンを20に設定
        
        string html = "<html><body><h1>Custom Page Settings</h1></body></html>"; // HTML文字列を定義
        PdfDocument doc = converter.ConvertHtmlString(html); // HTML文字列からPDFを生成
        doc.Save("custom-settings.pdf"); // PDFをファイルに保存
        doc.Close(); // PDFドキュメントを閉じる
        
        Console.WriteLine("PDF with custom settings created"); // カスタム設定のPDFが作成されたことを出力
    }
}
```