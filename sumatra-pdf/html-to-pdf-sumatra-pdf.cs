```csharp
// NuGet: Install-Package SumatraPDF (注: Sumatraは主にビューアであり、生成器ではありません)
// Sumatra PDFはHTMLからPDFへの直接変換をC#で直接行う機能がありません
// 外部ツールやライブラリを使用してからSumatraで開く必要があります
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main()
    {
        // Sumatra PDFはHTMLをPDFに直接変換できません
        // wkhtmltopdfまたは類似のツールを使用してから、Sumatraで表示する必要があります
        string htmlFile = "input.html";
        string pdfFile = "output.pdf";
        
        // 中間者としてwkhtmltopdfを使用
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "wkhtmltopdf.exe",
            Arguments = $"{htmlFile} {pdfFile}",
            UseShellExecute = false
        };
        Process.Start(psi)?.WaitForExit();
        
        // その後、Sumatraで開く
        Process.Start("SumatraPDF.exe", pdfFile);
    }
}
```