```csharp
// NuGet: Winnovative.WebToPdfConverterをインストール
using Winnovative;
using System;

class Program
{
    static void Main()
    {
        // HTMLからPDFへのコンバーターを作成する
        HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();
        
        // ライセンスキーを設定する
        htmlToPdfConverter.LicenseKey = "your-license-key";
        
        // URLをPDFに変換する
        string url = "https://www.example.com";
        byte[] pdfBytes = htmlToPdfConverter.ConvertUrl(url);
        
        // ファイルに保存する
        System.IO.File.WriteAllBytes("webpage.pdf", pdfBytes);
        
        Console.WriteLine("PDF from URL created successfully");
    }
}
```