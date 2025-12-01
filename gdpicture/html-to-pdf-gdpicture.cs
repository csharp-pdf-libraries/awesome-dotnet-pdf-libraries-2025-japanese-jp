```csharp
// NuGet: Install-Package GdPicture.NET
using GdPicture14;
using System;

class Program
{
    static void Main()
    {
        using (GdPictureDocumentConverter converter = new GdPictureDocumentConverter())
        {
            string htmlContent = "<html><body><h1>Hello World</h1></body></html>";
            GdPictureStatus status = converter.LoadFromHTMLString(htmlContent);
            
            if (status == GdPictureStatus.OK)
            {
                // GdPictureStatusがOKの場合、PDFとして保存します
                converter.SaveAsPDF("output.pdf");
            }
        }
    }
}
```