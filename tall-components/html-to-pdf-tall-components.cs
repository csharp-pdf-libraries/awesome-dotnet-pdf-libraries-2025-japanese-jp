```csharp
// NuGet: Install-Package TallComponents.PDF.Kit
using TallComponents.PDF.Kit;
using System.IO;

class Program
{
    static void Main()
    {
        // 新しいドキュメントを作成
        using (Document document = new Document())
        {
            string html = "<html><body><h1>Hello World</h1><p>This is a PDF from HTML.</p></body></html>";
            
            // HTMLフラグメントを作成
            Fragment fragment = Fragment.FromText(html);
            
            // ドキュメントに追加
            Section section = document.Sections.Add();
            section.Fragments.Add(fragment);
            
            // ファイルに保存
            using (FileStream fs = new FileStream("output.pdf", FileMode.Create))
            {
                document.Write(fs);
            }
        }
    }
}
```