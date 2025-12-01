```csharp
// NuGet: Winnovative.WebToPdfConverterをインストール
using Winnovative;
using System;

class Program
{
    static void Main()
    {
        // HTMLからPDFへのコンバーターを作成
        HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();
        
        // ライセンスキーを設定
        htmlToPdfConverter.LicenseKey = "your-license-key";
        
        // HTML文字列をPDFに変換
        string htmlString = "<html><body><h1>Hello World</h1></body></html>";
        byte[] pdfBytes = htmlToPdfConverter.ConvertHtml(htmlString, "");
        
        // ファイルに保存
        System.IO.File.WriteAllBytes("output.pdf", pdfBytes);
        
        Console.WriteLine("PDF created successfully");
    }
}
```