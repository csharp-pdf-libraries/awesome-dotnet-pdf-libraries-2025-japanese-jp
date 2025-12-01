```csharp
// Adobe PDF Library SDK
using Datalogics.PDFL;
using System;

class AdobeHtmlToPdf
{
    static void Main()
    {
        using (Library lib = new Library())
        {
            // Adobe PDF LibraryはHTML変換パラメータを使った複雑なセットアップが必要です
            HTMLConversionParameters htmlParams = new HTMLConversionParameters();
            htmlParams.PaperSize = PaperSize.Letter;
            htmlParams.Orientation = Orientation.Portrait;
            
            string htmlContent = "<html><body><h1>Hello World</h1></body></html>";
            
            // HTMLをPDFに変換
            Document doc = Document.CreateFromHTML(htmlContent, htmlParams);
            doc.Save(SaveFlags.Full, "output.pdf");
            doc.Dispose();
        }
    }
}
```