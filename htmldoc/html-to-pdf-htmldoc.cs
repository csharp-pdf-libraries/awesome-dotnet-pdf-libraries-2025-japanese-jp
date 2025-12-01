```csharp
// HTMLDOC コマンドラインアプローチ
using System.Diagnostics;

class HtmlDocExample
{
    static void Main()
    {
        // HTMLDOC は外部実行ファイルが必要です
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "htmldoc",
            Arguments = "--webpage -f output.pdf input.html",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };
        
        Process process = Process.Start(startInfo);
        process.WaitForExit();
    }
}
```