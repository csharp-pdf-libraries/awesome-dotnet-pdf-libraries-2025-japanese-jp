```csharp
// Sumatra PDFはテキスト抽出のためのC# APIを提供していません
// コマンドラインツールや他のライブラリを使用する必要があります
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main()
    {
        // Sumatra PDFはビューアであり、テキスト抽出ライブラリではありません
        // 抽出のためにはPDFBox、iTextSharp、または類似のものを使用する必要があります
        
        string pdfFile = "document.pdf";
        
        // これにはpdftotextのような外部ツールが必要です
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "pdftotext.exe",
            Arguments = $"{pdfFile} output.txt",
            UseShellExecute = false
        };
        
        Process.Start(psi)?.WaitForExit();
        
        string extractedText = File.ReadAllText("output.txt");
        Console.WriteLine(extractedText);
    }
}
```