```csharp
// NuGet: Install-Package TallComponents.PDF.Kit
using TallComponents.PDF.Kit;
using System.IO;

class Program
{
    static void Main()
    {
        // 出力ドキュメントを作成
        using (Document outputDoc = new Document())
        {
            // 最初のPDFをロード
            using (FileStream fs1 = new FileStream("document1.pdf", FileMode.Open))
            using (Document doc1 = new Document(fs1))
            {
                foreach (Page page in doc1.Pages)
                {
                    outputDoc.Pages.Add(page.Clone());
                }
            }
            
            // 2番目のPDFをロード
            using (FileStream fs2 = new FileStream("document2.pdf", FileMode.Open))
            using (Document doc2 = new Document(fs2))
            {
                foreach (Page page in doc2.Pages)
                {
                    outputDoc.Pages.Add(page.Clone());
                }
            }
            
            // マージしたドキュメントを保存
            using (FileStream output = new FileStream("merged.pdf", FileMode.Create))
            {
                outputDoc.Write(output);
            }
        }
    }
}
```