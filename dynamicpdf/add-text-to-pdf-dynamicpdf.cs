```csharp
// NuGet: Install-Package ceTe.DynamicPDF.CoreSuite.NET
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;

class Program
{
    static void Main()
    {
        Document document = new Document();
        Page page = new Page(PageSize.Letter);
        Label label = new Label("Hello from DynamicPDF!", 0, 0, 504, 100);
        page.Elements.Add(label); // ページ要素にラベルを追加
        document.Pages.Add(page); // ドキュメントページにページを追加
        document.Draw("output.pdf"); // "output.pdf"としてドキュメントを描画
    }
}
```