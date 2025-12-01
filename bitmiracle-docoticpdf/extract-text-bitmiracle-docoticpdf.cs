```csharp
// NuGet: Install-Package Docotic.Pdf
using BitMiracle.Docotic.Pdf;
using System;

class Program
{
    static void Main()
    {
        using (var pdf = new PdfDocument("document.pdf"))
        {
            string allText = "";
            
            foreach (var page in pdf.Pages)
            {
                // ページからテキストを抽出し、それを allText に追加します
                allText += page.GetText();
            }
            
            // 抽出されたテキストを表示します：
            Console.WriteLine("Extracted text:");
            Console.WriteLine(allText);
        }
    }
}
```