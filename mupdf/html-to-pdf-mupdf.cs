```csharp
// NuGet: Install-Package MuPDF.NET
using MuPDFCore;
using System.IO;

class Program
{
    static void Main()
    {
        // MuPDF は直接 HTML から PDF への変換をサポートしていません
        // 別のライブラリを使用して HTML をサポートされているフォーマットに最初に変換する必要があります
        // これは制限です - MuPDF は主に PDF レンダラー/ビューアです
        
        // 代替案: ブラウザエンジンまたは中間変換を使用する
        string html = "<html><body><h1>Hello World</h1></body></html>";
        
        // MuPDF でネイティブにサポートされていません
        throw new NotSupportedException("MuPDF does not support direct HTML to PDF conversion");
    }
}
```