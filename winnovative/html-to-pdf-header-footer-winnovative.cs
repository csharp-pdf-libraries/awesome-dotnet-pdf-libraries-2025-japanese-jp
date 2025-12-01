```csharp
// NuGet: Install-Package Winnovative.WebToPdfConverter
using Winnovative;
using System;
using System.Drawing;

class Program
{
    static void Main()
    {
        // HTMLからPDFへのコンバータを作成します
        HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();
        
        // ライセンスキーを設定します
        htmlToPdfConverter.LicenseKey = "your-license-key";
        
        // ヘッダーを有効にします
        htmlToPdfConverter.PdfDocumentOptions.ShowHeader = true;
        htmlToPdfConverter.PdfHeaderOptions.HeaderHeight = 60;
        
        // ヘッダーテキストを追加します
        TextElement headerText = new TextElement(0, 0, "Document Header", new Font("Arial", 12));
        htmlToPdfConverter.PdfHeaderOptions.AddElement(headerText);
        
        // フッターを有効にします
        htmlToPdfConverter.PdfDocumentOptions.ShowFooter = true;
        htmlToPdfConverter.PdfFooterOptions.FooterHeight = 60;
        
        // ページ番号を含むフッターを追加します
        TextElement footerText = new TextElement(0, 0, "Page &p; of &P;", new Font("Arial", 10));
        htmlToPdfConverter.PdfFooterOptions.AddElement(footerText);
        
        // HTMLをPDFに変換します
        string htmlString = "<html><body><h1>Document with Header and Footer</h1><p>Content goes here</p></body></html>";
        byte[] pdfBytes = htmlToPdfConverter.ConvertHtml(htmlString, "");
        
        // ファイルに保存します
        System.IO.File.WriteAllBytes("document.pdf", pdfBytes);
        
        Console.WriteLine("PDF with header and footer created successfully");
    }
}
```