```csharp
// NuGet: Install-Package Fluid.Core
using Fluid;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var parser = new FluidParser();
        var template = parser.Parse(@"
            <html><body>
                <h1>{{title}}</h1>
                <ul>
                {% for item in items %}
                    <li>{{item}}</li>
                {% endfor %}
                </ul>
            </body></html>");
        
        var context = new TemplateContext();
        context.SetValue("title", "My List");
        context.SetValue("items", new[] { "Item 1", "Item 2", "Item 3" });
        
        var html = await template.RenderAsync(context);
        // FluidはHTMLのみを生成します - PDFへの変換は別途必要です
        File.WriteAllText("template-output.html", html);
    }
}
```