```csharp
// NuGet: Install-Package SumatraPDF.CommandLine (または直接実行可能ファイル)
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main()
    {
        string pdfPath = "document.pdf";
        
        // Sumatra PDF は PDF の閲覧に優れています
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "SumatraPDF.exe",
            Arguments = $"\"{pdfPath}\"",
            UseShellExecute = true
        };
        
        Process.Start(startInfo);
        
        // 任意: 特定のページを開く
        // Arguments = $"-page 5 \"{pdfPath}\""
    }
}
```