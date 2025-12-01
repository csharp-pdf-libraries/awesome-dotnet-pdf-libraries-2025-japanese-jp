```csharp
// NuGet: Install-Package GdPicture.NET
using GdPicture14;
using System;

class Program
{
    static void Main()
    {
        using (GdPicturePDF pdf1 = new GdPicturePDF())
        using (GdPicturePDF pdf2 = new GdPicturePDF())
        {
            pdf1.LoadFromFile("document1.pdf", false); // "document1.pdf"を読み込む、falseで読み込みモードを指定
            pdf2.LoadFromFile("document2.pdf", false); // "document2.pdf"を読み込む、falseで読み込みモードを指定
            
            pdf1.MergePages(pdf2); // pdf2のページをpdf1にマージする
            pdf1.SaveToFile("merged.pdf"); // "merged.pdf"として保存する
        }
    }
}
```