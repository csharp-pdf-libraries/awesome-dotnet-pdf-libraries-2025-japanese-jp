```csharp
// NuGet: Install-Package Syncfusion.Pdf.Net.Core
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System.IO;

class Program
{
    static void Main()
    {
        // HTMLからPDFへのコンバータを初期化
        HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
        
        // URLをPDFに変換
        PdfDocument document = htmlConverter.Convert("https://www.example.com");
        
        // ドキュメントを保存
        FileStream fileStream = new FileStream("Output.pdf", FileMode.Create);
        document.Save(fileStream);
        document.Close(true);
        fileStream.Close();
    }
}
```