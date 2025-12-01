```csharp
// NuGet: Install-Package Ghostscript.NET
using Ghostscript.NET;
using Ghostscript.NET.Processor;
using System.IO;
using System.Text;

class GhostscriptExample
{
    static void Main()
    {
        // GhostscriptはHTMLをPDFに直接変換できません
        // まず別のツールを使用してHTMLをPS/EPSに変換し、
        // その後Ghostscriptを使用してPSからPDFに変換する必要があります
        
        string htmlContent = "<html><body><h1>Hello World</h1></body></html>";
        string psFile = "temp.ps";
        string outputPdf = "output.pdf";
        
        // これは回避策です - Ghostscriptは主にPostScriptを扱います
        GhostscriptProcessor processor = new GhostscriptProcessor();
        
        List<string> switches = new List<string>
        {
            "-dNOPAUSE",
            "-dBATCH",
            "-dSAFER",
            "-sDEVICE=pdfwrite",
            $"-sOutputFile={outputPdf}",
            psFile
        };
        
        processor.Process(switches.ToArray());
    }
}
```