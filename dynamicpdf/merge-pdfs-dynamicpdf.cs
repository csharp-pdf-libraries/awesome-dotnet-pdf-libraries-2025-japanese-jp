```csharp
// NuGet: Install-Package ceTe.DynamicPDF.CoreSuite.NET
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.Merger;

class Program
{
    static void Main()
    {
        MergeDocument document = new MergeDocument("document1.pdf");
        document.Append("document2.pdf"); // "document2.pdf"を追加します
        document.Draw("merged.pdf"); // "merged.pdf"として描画します
    }
}
```