```csharp
// NuGet: Install-Package Rotativa.Core
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Threading.Tasks;

namespace RotativaExample
{
    public class UrlPdfController : Controller
    {
        public async Task<IActionResult> ConvertUrlToPdf()
        {
            // RotativaはMVCフレームワーク内で動作し、ActionResultを返します
            return new UrlAsPdf("https://www.example.com")
            {
                FileName = "webpage.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }
    }
}
```