```csharp
// NuGet: Install-Package GrabzIt
using GrabzIt;
using GrabzIt.Parameters;
using System;

class Program
{
    static void Main()
    {
        var grabzIt = new GrabzItClient("YOUR_APPLICATION_KEY", "YOUR_APPLICATION_SECRET");
        var options = new PDFOptions();
        options.PageSize = PageSize.A4;
        
        // 指定されたURLからPDFを生成します
        grabzIt.URLToPDF("https://www.example.com", options);
        // 結果をファイルに保存します
        grabzIt.SaveTo("webpage.pdf");
    }
}
```