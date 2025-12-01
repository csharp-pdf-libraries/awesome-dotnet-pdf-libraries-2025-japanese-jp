```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        var pdf = PdfDocument.FromFile("document.pdf");
        
        // 情報を抽出する
        Console.WriteLine($"Page Count: {pdf.PageCount}");
        
        // IronPDFは操作して保存でき、デフォルトのビューアで開けます
        pdf.SaveAs("modified.pdf");
        
        // デフォルトのPDFビューアで開く
        Process.Start(new ProcessStartInfo("modified.pdf") { UseShellExecute = true });
    }
}
```