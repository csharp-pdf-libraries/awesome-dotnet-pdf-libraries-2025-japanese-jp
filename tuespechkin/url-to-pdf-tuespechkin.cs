```csharp
// NuGet: Install-Package TuesPechkin をインストール
using TuesPechkin;
using System.IO;

class Program
{
    static void Main()
    {
        var converter = new StandardConverter(
            new RemotingToolset<PdfToolset>(
                new Win64EmbeddedDeployment(
                    new TempFolderDeployment())));
        
        byte[] pdfBytes = converter.Convert(new HtmlToPdfDocument
        {
            Objects = {
                new ObjectSettings {
                    PageUrl = "https://www.example.com"
                }
            }
        });
        
        File.WriteAllBytes("webpage.pdf", pdfBytes); // "webpage.pdf" として PDF バイトを書き込む
    }
}
```