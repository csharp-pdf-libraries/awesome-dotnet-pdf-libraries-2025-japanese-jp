```csharp
// NuGet: Install-Package PdfiumViewer
using PdfiumViewer;
using System.IO;
using System.Drawing.Printing;

// 注意: PdfiumViewerは主にPDFの表示/レンダリング用であり、HTMLからPDFを作成するためのものではありません
// Pdfium.NETでHTMLからPDFにするには追加のライブラリが必要です
// この例はPdfium.NETの制限を示しています
class Program
{
    static void Main()
    {
        // Pdfium.NETにはネイティブのHTMLからPDFへの変換機能がありません
        // HTMLをPDFに変換するためには別のライブラリを使用する必要があります
        // その後でPdfiumを使用して操作します
        string htmlContent = "<h1>Hello World</h1>";
        
        // この機能はPdfium.NETでは直接利用できません
        Console.WriteLine("HTML to PDF conversion not natively supported in Pdfium.NET");
    }
}
```