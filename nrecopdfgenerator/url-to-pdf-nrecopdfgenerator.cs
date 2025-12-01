```csharp
// NuGet: Install-Package NReco.PdfGenerator
using NReco.PdfGenerator;
using System.IO;

class Program
{
    static void Main()
    {
        var htmlToPdf = new HtmlToPdfConverter();
        // "https://www.example.com" から PDF を生成
        var pdfBytes = htmlToPdf.GeneratePdfFromFile("https://www.example.com", null);
        // "webpage.pdf" として PDF バイトをファイルに書き込む
        File.WriteAllBytes("webpage.pdf", pdfBytes);
    }
}
```