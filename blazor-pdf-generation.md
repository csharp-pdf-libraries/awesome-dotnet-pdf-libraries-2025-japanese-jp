---
** ** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/blazor-pdf-generation.md)

---
# Blazor PDF生成：サーバー、WebAssembly、およびMAUIハイブリッドガイド

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFのクリエーター

[![Blazor](https://img.shields.io/badge/Blazor-Server%20%7C%20WASM%20%7C%20Hybrid-512BD4)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![Last Updated](https://img.shields.io/badge/Updated-November%202025-green)]()

> Blazor開発者は、レポート、請求書、およびエクスポートのためにPDF生成を必要とします。このガイドは、サーバー、WebAssembly、およびMAUIハイブリッドを含むすべてのBlazorホスティングモデルをカバーし、実際の例を提供します。

---

## 目次

1. [BlazorホスティングモデルとPDF](#blazor-hosting-models-and-pdf)
2. [クイックスタート](#quick-start)
3. [Blazorサーバー](#blazor-server)
4. [Blazor WebAssembly](#blazor-webassembly)
5. [Blazor MAUIハイブリッド](#blazor-maui-hybrid)
6. [RazorコンポーネントをPDFにレンダリング](#render-razor-components-to-pdf)
7. [BlazorからPDFをダウンロード](#download-pdfs-from-blazor)
8. [共通パターン](#common-patterns)

---

## BlazorホスティングモデルとPDF

| ホスティングモデル | PDF生成場所 | アプローチ |
|--------------|------------------------|----------|
| **Blazorサーバー** | サーバー側 | IronPDFの直接使用 |
| **Blazor WebAssembly** | サーバーAPI | APIエンドポイントの呼び出し |
| **Blazor MAUIハイブリッド** | デバイス | IronPDFの直接使用 |
| **Blazor United (.NET 8)** | サーバー | 直接またはAPI |

**重要な洞察:** WebAssemblyはブラウザ内で実行され、直接IronPDFを使用できません—サーバーAPIが必要です。

---

## クイックスタート

### IronPDFのインストール

```bash
dotnet add package IronPdf
```

### 基本的なPDF生成

```csharp
using IronPdf;

var pdf = ChromePdfRenderer.RenderHtmlAsPdf("<h1>Hello from Blazor!</h1>");
pdf.SaveAs("hello.pdf");
```

---

## Blazorサーバー

Blazorサーバーはサーバー上で実行されるため、IronPDFは直接動作します。

### サービス登録

```csharp
// Program.cs
builder.Services.AddScoped<IPdfService, PdfService>();
```

### PDFサービス

```csharp
// Services/PdfService.cs
using IronPdf;

public interface IPdfService
{
    byte[] GeneratePdf(string html);
    byte[] GenerateInvoice(InvoiceModel invoice);
}

public class PdfService : IPdfService
{
    private readonly ChromePdfRenderer _renderer;

    public PdfService()
    {
        _renderer = new ChromePdfRenderer();
        _renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        _renderer.RenderingOptions.MarginTop = 20;
        _renderer.RenderingOptions.MarginBottom = 20;
    }

    public byte[] GeneratePdf(string html)
    {
        return _renderer.RenderHtmlAsPdf(html).BinaryData;
    }

    public byte[] GenerateInvoice(InvoiceModel invoice)
    {
        string html = BuildInvoiceHtml(invoice);
        return _renderer.RenderHtmlAsPdf(html).BinaryData;
    }

    private string BuildInvoiceHtml(InvoiceModel invoice)
    {
        return $@"
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; }}
                .header {{ display: flex; justify-content: space-between; }}
                .items {{ width: 100%; border-collapse: collapse; }}
                .items th, .items td {{ border: 1px solid #ddd; padding: 8px; }}
            </style>
        </head>
        <body>
            <div class='header'>
                <h1>Invoice #{invoice.Number}</h1>
                <p>Date: {invoice.Date:yyyy-MM-dd}</p>
            </div>
            <table class='items'>
                <tr><th>Description</th><th>Amount</th></tr>
                {string.Join("", invoice.Items.Select(i => $"<tr><td>{i.Description}</td><td>{i.Amount:C}</td></tr>"))}
            </table>
            <p><strong>Total: {invoice.Total:C}</strong></p>
        </body>
        </html>";
    }
}
```

### Blazorコンポーネント

```razor
@* Pages/InvoiceReport.razor *@
@page "/invoice/{InvoiceId:int}"
@inject IPdfService PdfService
@inject IJSRuntime JS

<h3>Invoice #@Invoice?.Number</h3>

<button class="btn btn-primary" @onclick="DownloadPdf">
    PDFをダウンロード
</button>

@code {
    [Parameter] public int InvoiceId { get; set; }
    private InvoiceModel Invoice { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Invoice = await LoadInvoice(InvoiceId);
    }

    private async Task DownloadPdf()
    {
        var pdfBytes = PdfService.GenerateInvoice(Invoice);

        // JavaScriptを介してダウンロードをトリガー
        await JS.InvokeVoidAsync("downloadFile",
            $"invoice-{Invoice.Number}.pdf",
            Convert.ToBase64String(pdfBytes));
    }
}
```

### JavaScriptダウンロードヘルパー

```javascript
// wwwroot/js/download.js
window.downloadFile = (fileName, base64Data) => {
    const link = document.createElement('a');
    link.href = 'data:application/pdf;base64,' + base64Data;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
```

---

## Blazor WebAssembly

WebAssemblyはブラウザ内で実行されるため、PDF生成はサーバー側で行わなければなりません。

### サーバーAPIエンドポイント

```csharp
// Server/Controllers/PdfController.cs
[ApiController]
[Route("api/[controller]")]
public class PdfController : ControllerBase
{
    [HttpPost("generate")]
    IActionResult GeneratePdf([FromBody] PdfRequest request)
    {
        var pdf = ChromePdfRenderer.RenderHtmlAsPdf(request.Html);
        return File(pdf.BinaryData, "application/pdf", request.FileName);
    }

    [HttpGet("invoice/{id}")]
    async Task<IActionResult> GetInvoicePdf(int id)
    {
        var invoice = await _invoiceService.GetById(id);
        string html = GenerateInvoiceHtml(invoice);

        var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);
        return File(pdf.BinaryData, "application/pdf", $"invoice-{id}.pdf");
    }
}
```

### クライアントサービス

```csharp
// Client/Services/PdfClientService.cs
public class PdfClientService
{
    private readonly HttpClient _http;

    public PdfClientService(HttpClient http)
    {
        _http = http;
    }

    async Task<byte[]> GeneratePdf(string html, string fileName)
    {
        var response = await _http.PostAsJsonAsync("api/pdf/generate", new
        {
            Html = html,
            FileName = fileName
        });

        return await response.Content.ReadAsByteArrayAsync();
    }

    async Task DownloadInvoice(int invoiceId)
    {
        var response = await _http.GetAsync($"api/pdf/invoice/{invoiceId}");
        var bytes = await response.Content.ReadAsByteArrayAsync();

        // ダウンロードをトリガー
        await DownloadFileFromBytes($"invoice-{invoiceId}.pdf", bytes);
    }
}
```

### クライアントコンポーネント

```razor
@* Client/Pages/InvoiceDownload.razor *@
@page "/invoice/{InvoiceId:int}"
@inject PdfClientService PdfService
@inject IJSRuntime JS

<h3>Invoice</h3>

<button class="btn btn-primary" @onclick="DownloadPdf" disabled="@IsLoading">
    @if (IsLoading)
    {
        <span class="spinner-border spinner-border-sm"></span>
        <span>生成中...</span>
    }
    else
    {
        <span>PDFをダウンロード</span>
    }
</button>

@code {
    [Parameter] public int InvoiceId { get; set; }
    private bool IsLoading { get; set; }

    async Task DownloadPdf()
    {
        IsLoading = true;
        try
        {
            await PdfService.DownloadInvoice(InvoiceId);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

---

## Blazor MAUIハイブリッド

MAUIハイブリッドアプリはネイティブに実行されるため、IronPDFはデバイス上で直接動作します。

### MAUI用PDFサービス

```csharp
// Services/MauiPdfService.cs
using IronPdf;

public class MauiPdfService
{
    async Task<string> GenerateAndSavePdf(string html, string fileName)
    {
        // モバイル用に設定
        IronPdf.Installation.ChromeGpuMode = IronPdf.Engines.Chrome.ChromeGpuModes.Disabled;

        var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);

        // アプリ固有のフォルダに保存
        string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
        pdf.SaveAs(filePath);

        return filePath;
    }

    async Task SharePdf(string filePath)
    {
        await Share.RequestAsync(new ShareFileRequest
        {
            Title = "PDFを共有",
            File = new ShareFile(filePath)
        });
    }
}
```

### MAUI Blazorコンポーネント

```razor
@* Components/ReportGenerator.razor *@
@inject MauiPdfService PdfService

<button @onclick="GenerateAndShare">レポートを生成して共有</button>

@code {
    async Task GenerateAndShare()
    {
        string html = @"
        <html>
        <body>
            <h1>モバイルレポート</h1>
            <p>生成日: " + DateTime.Now + @"</p>
        </body>
        </html>";

        string filePath = await PdfService.GenerateAndSavePdf(html, "report.pdf");
        await PdfService.SharePdf(filePath);
    }
}
```

---

## RazorコンポーネントをPDFにレンダリング

テンプレートを複製することなく、既存のRazorコンポーネントをPDFに変換します。

### コンポーネントからHTMLへのレンダラー

```csharp
// Services/RazorComponentRenderer.cs
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

public class RazorComponentRenderer
{
    private readonly IServiceProvider _serviceProvider;

    public RazorComponentRenderer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    async Task<string> RenderComponentAsync<TComponent>(
        Dictionary<string, object> parameters = null) where TComponent : IComponent
    {
        await using var htmlRenderer = new HtmlRenderer(_serviceProvider, NullLoggerFactory.Instance);

        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var parameterView = parameters != null
                ? ParameterView.FromDictionary(parameters)
                : ParameterView.Empty;

            var output = await htmlRenderer.RenderComponentAsync<TComponent>(parameterView);
            return output.ToHtmlString();
        });

        return html;
    }
}
```

### 使用例

```csharp
// 既存のRazorコンポーネントからPDFを生成
var renderer = new RazorComponentRenderer(_serviceProvider);

string html = await renderer.RenderComponentAsync<InvoiceComponent>(new Dictionary<string, object>
{
    { "Invoice", invoiceModel }
});

var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);
```

---

## BlazorからPDFをダウンロード

### JavaScriptインタロップを使用

```javascript
// wwwroot/js/blazor-pdf.js
window.blazorPdf = {
    downloadFromBase64: function(fileName, base64) {
        const link = document.createElement('a');
        link.href = 'data:application/pdf;base64,' + base64;
        link.download = fileName;
        link.click();
    },

    downloadFromUrl: function(url, fileName) {
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName;
        link.click();
    },

    openInNewTab: function(base64) {
        const blob = this.base64ToBlob(base64, 'application/pdf');
        const url = URL.createObjectURL(blob);
        window.open(url, '_blank');
    },

    base64ToBlob: function(base64, contentType) {
        const byteCharacters = atob(base64);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        return new Blob([byteArray], { type: contentType });
    }
};
```

### Blazorコンポーネントラッパー

```razor
@* Shared/PdfDownloader.razor *@
@inject IJSRuntime JS

@code {
    public async Task DownloadPdf(byte[] pdfBytes, string fileName)
    {
        var base64 = Convert.ToBase64String(pdfBytes);
        await JS.InvokeVoidAsync("blazorPdf.downloadFromBase64", fileName, base64);
    }

    public async Task OpenPdfInNewTab(byte[] pdfBytes)
    {
        var base64 = Convert.ToBase64String(pdfBytes);
        await JS.InvokeVoidAsync("blazorPdf.openInNewTab", base64);
    }
}
```

---

## 共通パターン

### チャート付きレポート（Blazorサーバー）

```razor
@page "/sales-report"
@inject IPdfService PdfService
@inject IJSRuntime JS

<div id="chart-container">
    <ApexChart TItem="SalesData" ... />
</div>

<button @onclick="DownloadReport">PDFレポートをダウンロード</button>

@code {
    private async Task DownloadReport()
    {
        // チャートのレンダリングを待つ
        await Task.Delay(500);

        // JavaScriptを介してチャートを画像として取得
        var chartImage = await JS.InvokeAsync<string>("getChartAsBase64");

        string html = $@"
        <html>
        <body>
            <h1>売上レポート</h1>
            <img src='data:image/png;base64,{chartImage}' />
            <table>
                <!-- 売上データテーブル -->
            </table>
        </body>
        </html>";

        var pdf = PdfService.GeneratePdf(html);
        await DownloadPdfBytes(pdf, "sales-report.pdf");
    }
}
```

### マルチページレポート

```csharp
public byte[] GenerateMultiPageReport(List<ReportSection> sections)
{
    var html = new StringBuilder();
    html.Append("<html><head><style>");
    html.Append(".page-break { page-break-before: always; }");
    html.Append("</style></head><body>");

    for (int i = 0; i < sections.Count; i++)
    {
        if (i > 0) html.Append("<div class='page-break'></div>");
        html.Append($"<h1>{sections[i].Title}</h1>");
        html.Append(sections[i].Content);
    }

    html.Append("</body></html>");

    return ChromePdfRenderer.RenderHtmlAsPdf(html.ToString()).BinaryData;
}
```

---

## 推奨事項

### Blazorサーバー向け:
- ✅ IronPDFを直接使用
- ✅ シンプルなサービス注入
- ✅ 全機能へのアクセス

### Blazor WebAssembly向け:
- ✅ サーバーAPIエンドポイントを作成
- ✅ HttpClientを使用してAPIを呼び出し
- ✅ ローディング状態の処理

### Blazor MAUI向け:
- ✅ IronPDFを直接使用
- ✅ モバイル用にGPUモードを無効化
- ✅ 配布用にShare APIを使用

---

## 関連チュートリアル

- **[ASP.NET Core PDF](asp-net-core-pdf-reports.md)** — MVC/API PDF生成
- **[HTMLからPDFへ](html-to-pdf-csharp.md)** — HTMLの包括的ガイド
- **[クロスプラットフォームPDF](cross-platform-pdf-dotnet.md)** — デプロイメントガイド
- **[最高のPDFライブラリ](best-pdf-libraries-dotnet-2025.md)** — 完全な比較