```csharp
// NuGet: Install-Package PDFBolt をインストール
using PDFBolt;
using System.IO;

class Program
{
    static void Main()
    {
        var converter = new HtmlToPdfConverter();
        var pdf = converter.ConvertUrl("https://www.example.com");
        File.WriteAllBytes("webpage.pdf", pdf);
    }
}
```