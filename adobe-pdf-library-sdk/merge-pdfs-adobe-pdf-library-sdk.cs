```csharp
// Adobe PDF Library SDK
using Datalogics.PDFL;
using System;

class AdobeMergePdfs
{
    static void Main()
    {
        using (Library lib = new Library())
        {
            // 最初のPDFドキュメントを開く
            Document doc1 = new Document("document1.pdf");
            Document doc2 = new Document("document2.pdf");
            
            // 2番目のドキュメントからページを最初のドキュメントに挿入する
            PageInsertParams insertParams = new PageInsertParams();
            insertParams.InsertFlags = PageInsertFlags.None;
            
            for (int i = 0; i < doc2.NumPages; i++)
            {
                Page page = doc2.GetPage(i);
                doc1.InsertPage(doc1.NumPages - 1, page, insertParams);
            }
            
            doc1.Save(SaveFlags.Full, "merged.pdf");
            doc1.Dispose();
            doc2.Dispose();
        }
    }
}
```