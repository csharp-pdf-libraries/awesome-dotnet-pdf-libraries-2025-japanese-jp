```csharp
// NuGet: Install-Package MuPDF.NET をインストール
using MuPDFCore;
using System.IO;

class Program
{
    static void Main()
    {
        using (MuPDFDocument doc1 = new MuPDFDocument("file1.pdf"))
        using (MuPDFDocument doc2 = new MuPDFDocument("file2.pdf"))
        {
            // 新しいドキュメントを作成
            using (MuPDFDocument mergedDoc = MuPDFDocument.Create())
            {
                // 最初のドキュメントからページをコピー
                for (int i = 0; i < doc1.Pages.Count; i++)
                {
                    mergedDoc.CopyPage(doc1, i);
                }
                
                // 二番目のドキュメントからページをコピー
                for (int i = 0; i < doc2.Pages.Count; i++)
                {
                    mergedDoc.CopyPage(doc2, i);
                }
                
                mergedDoc.Save("merged.pdf");
            }
        }
    }
}
```