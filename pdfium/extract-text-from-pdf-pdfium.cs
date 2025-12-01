```csharp
// NuGet: Install-Package PdfiumViewer
using PdfiumViewer;
using System;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        string pdfPath = "document.pdf";
        
        using (var document = PdfDocument.Load(pdfPath))
        {
            StringBuilder text = new StringBuilder();
            
            for (int i = 0; i < document.PageCount; i++)
            {
                // 注意: PdfiumViewerは限定的なテキスト抽出機能しか持っていません
                // テキスト抽出にはPdfium.NETでの追加作業が必要です
                string pageText = document.GetPdfText(i);
                text.AppendLine(pageText);
            }
            
            Console.WriteLine(text.ToString());
        }
    }
}
```