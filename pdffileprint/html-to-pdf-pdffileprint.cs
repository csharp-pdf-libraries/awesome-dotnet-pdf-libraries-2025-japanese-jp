```csharp
// NuGet: Install-Package PDFFilePrint をインストール
using System;
using PDFFilePrint;

class Program
{
    static void Main()
    {
        var pdf = new PDFFile();
        string htmlContent = "<html><body><h1>Hello World</h1></body></html>";
        pdf.CreateFromHtml(htmlContent);
        pdf.SaveToFile("output.pdf");
    }
}
```