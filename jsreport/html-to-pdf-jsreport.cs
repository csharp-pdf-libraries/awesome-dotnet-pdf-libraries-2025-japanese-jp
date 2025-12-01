```csharp
// NuGet: jsreport.Binaryをインストール
// NuGet: jsreport.Localをインストール
// NuGet: jsreport.Typesをインストール
using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var rs = new LocalReporting()
            .UseBinary(JsReportBinary.GetBinary())
            .AsUtility()
            .Create();

        var report = await rs.RenderAsync(new RenderRequest()
        {
            Template = new Template()
            {
                Recipe = Recipe.ChromePdf,
                Engine = Engine.None,
                Content = "<h1>Hello from jsreport</h1><p>This is a PDF document.</p>"
            }
        });

        using (var fileStream = File.Create("output.pdf"))
        {
            report.Content.CopyTo(fileStream);
        }

        Console.WriteLine("PDF created successfully!");
    }
}
```