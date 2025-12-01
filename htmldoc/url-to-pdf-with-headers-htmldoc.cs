```csharp
// HTMLDOC コマンドライン URL とヘッダー
using System.Diagnostics;

class HtmlDocExample
{
    static void Main()
    {
        // HTMLDOC は URL とヘッダーのサポートが限定的です
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "htmldoc",
            Arguments = "--webpage --header \"Page #\" --footer \"t\" -f output.pdf https://example.com",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        
        Process process = Process.Start(startInfo);
        process.WaitForExit();
        
        // 注意: HTMLDOC は現代のウェブページを正しくレンダリングしないかもしれません
    }
}
```