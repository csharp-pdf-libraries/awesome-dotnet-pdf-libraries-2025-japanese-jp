```csharp
using PeachPDF;
using System.IO;

class Program
{
    static void Main()
    {
        var converter = new HtmlToPdfConverter();
        converter.Header = "<div style='text-align:center'>My Header</div>"; // ヘッダーを設定
        converter.Footer = "<div style='text-align:center'>Page {page}</div>"; // フッターを設定
        var html = "<html><body><h1>Document Content</h1></body></html>"; // HTMLコンテンツ
        var pdf = converter.Convert(html); // HTMLをPDFに変換
        File.WriteAllBytes("document.pdf", pdf); // PDFファイルを保存
    }
}
```