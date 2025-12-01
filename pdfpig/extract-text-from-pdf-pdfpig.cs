```csharp
// NuGet: Install-Package PdfPig をインストール
using UglyToad.PdfPig;
using System;
using System.Text;

class Program
{
    static void Main()
    {
        using (var document = PdfDocument.Open("input.pdf"))
        {
            var text = new StringBuilder();
            foreach (var page in document.GetPages())
            {
                text.AppendLine(page.Text);
            }
            Console.WriteLine(text.ToString());
        }
    }
}
```