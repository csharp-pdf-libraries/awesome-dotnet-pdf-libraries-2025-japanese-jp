```csharp
// NuGet: Install-Package IronPdf を使用
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        // HTML文字列からPDFを作成
        var renderer = new ChromePdfRenderer();
        
        string htmlContent = "<h1>Hello World</h1><p>This is a PDF generated from HTML.</p>";
        
        var pdf = renderer.RenderHtmlAsPdf(htmlContent);
        pdf.SaveAs("output.pdf");
        
        Console.WriteLine("PDF created successfully!");
    }
}
```