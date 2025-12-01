---
**🌐 日本語版** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/asp-net-core-pdf-reports.md)

---

# ASP.NET CoreでPDFレポートを生成する：完全ガイド

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFのクリエーター

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4)](https://dotnet.microsoft.com/apps/aspnet)
[![最終更新](https://img.shields.io/badge/Updated-November%202025-green)]()

> ASP.NET CoreでのPDF生成は、多くのライブラリが不安定な出力を生成したり、アクセシビリティ監査に失敗したり、レイテンシを追加するクラウドサービスが必要だったりすることを知るまでは単純に見えます。このガイドでは、本番環境に適したアプローチを示します。

---

## 目次

1. [ASP.NET Core PDFの課題](#aspnet-core-pdfの課題)
2. [クイックスタート](#クイックスタート)
3. [RazorビューをPDFにレンダリング](#razorビューをpdfにレンダリング)
4. [コントローラーアクション](#コントローラーアクション)
5. [最小APIエンドポイント](#最小apiエンドポイント)
6. [バックグラウンドでのPDF生成](#バックグラウンドでのpdf生成)
7. [ヘッダー、フッター、ページ番号](#ヘッダーフッターページ番号)
8. [PDF出力のためのスタイリング](#pdf出力のためのスタイリング)
9. [一般的なユースケース](#一般的なユースケース)
10. [本番環境での考慮事項](#本番環境での考慮事項)
11. [デプロイメント](#デプロイメント)

---

## ASP.NET Core PDFの課題

数千人のASP.NET Core開発者をサポートした後で学んだことは以下の通りです：

**素朴なアプローチは失敗します：**
1. チームは「シンプル」なPDFライブラリを選択します
2. 開発中は基本的なPDFが機能します
3. 本番環境では、欠けているフォント、壊れたレイアウト、アクセシビリティの失敗が明らかになります
4. セクション508/EU基準のコンプライアンス監査に失敗します
5. チームはライブラリの交換に慌てます

**クラウドアプローチは摩擦を追加します：**
1. チームはクラウドPDF APIを使用します
2. 機能しますが、PDFごとに2-5秒のレイテンシがあります
3. 顧客データが第三者のサーバーを流れます
4. コンプライアンスチームが懸念を提起します
5. 費用が月々積み上がります

**正しいアプローチ：** ローカルのChromiumレンダリングとIronPDF—高速、コンプライアンス、データはローカルに留まります。

---

## クイックスタート

### 1. IronPDFをインストール

```bash
dotnet add package IronPdf
```

### 2. 最初のPDFを生成

```csharp
using IronPdf;

var pdf = ChromePdfRenderer.RenderHtmlAsPdf("<h1>Hello from ASP.NET Core</h1>");
pdf.SaveAs("hello.pdf");
```

### 3. コントローラーからPDFを返す

```csharp
[HttpGet("invoice/{id}")]
public IActionResult GetInvoice(int id)
{
    var html = $"<h1>Invoice #{id}</h1><p>Thank you for your purchase!</p>";
    var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);

    return File(pdf.BinaryData, "application/pdf", $"invoice-{id}.pdf");
}
```

これで、PDFは完全なChromiumレンダリングで生成され、ダウンロードとして返されます。

---

## RazorビューをPDFにレンダリング

Razorビューとテンプレートを再利用することが真の力です。

### セットアップ：IRazorViewToStringRenderer

最初に、RazorビューをHTML文字列にレンダリングするサービスを作成します：

```csharp
// Services/IRazorViewToStringRenderer.cs
public interface IRazorViewToStringRenderer
{
    Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
}

// Services/RazorViewToStringRenderer.cs
public class RazorViewToStringRenderer : IRazorViewToStringRenderer
{
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public RazorViewToStringRenderer(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
    {
        var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        using var writer = new StringWriter();

        var viewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: false);

        if (!viewResult.Success)
        {
            throw new InvalidOperationException($"ビューが見つかりませんでした：{viewName}");
        }

        var viewDictionary = new ViewDataDictionary<TModel>(
            new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewDictionary,
            new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
            writer,
            new HtmlHelperOptions());

        await viewResult.View.RenderAsync(viewContext);
        return writer.ToString();
    }
}
```

### サービスを登録

```csharp
// Program.cs
builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
```

### Razorビューテンプレートを作成

```cshtml
@* Views/Pdf/InvoiceTemplate.cshtml *@
@model InvoiceViewModel

<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: 'Segoe UI', Arial, sans-serif; margin: 40px; }
        .header { display: flex; justify-content: space-between; margin-bottom: 40px; }
        .logo { height: 60px; }
        .invoice-details { text-align: right; }
        .items { width: 100%; border-collapse: collapse; margin: 20px 0; }
        .items th, .items td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }
        .items th { background-color: #f5f5f5; }
        .total { text-align: right; font-size: 1.2em; font-weight: bold; }
        .footer { margin-top: 40px; text-align: center; color: #666; font-size: 0.9em; }
    </style>
</head>
<body>
    <div class="header">
        <img src="https://yourcompany.com/logo.png" class="logo" alt="Company Logo" />
        <div class="invoice-details">
            <h1>Invoice #@Model.InvoiceNumber</h1>
            <p>Date: @Model.Date.ToString("MMMM dd, yyyy")</p>
            <p>Due: @Model.DueDate.ToString("MMMM dd, yyyy")</p>
        </div>
    </div>

    <div class="customer">
        <h3>Bill To:</h3>
        <p>@Model.CustomerName</p>
        <p>@Model.CustomerAddress</p>
        <p>@Model.CustomerEmail</p>
    </div>

    <table class="items">
        <thead>
            <tr>
                <th>Description</th>
                <th>Quantity</th>
                <th>Unit Price</th>
                <th>Amount</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var item in Model.Items)
            {
                <tr>
                    <td>@item.Description</td>
                    <td>@item.Quantity</td>
                    <td>@item.UnitPrice.ToString("C")</td>
                    <td>@item.Total.ToString("C")</td>
                </tr>
            }
        </tbody>
    </table>

    <div class="total">
        <p>Subtotal: @Model.Subtotal.ToString("C")</p>
        <p>Tax (@Model.TaxRate%): @Model.TaxAmount.ToString("C")</p>
        <p>Total: @Model.Total.ToString("C")</p>
    </div>

    <div class="footer">
        <p>ご利用ありがとうございます！</p>
        <p>質問がある場合は、billing@yourcompany.comまでお問い合わせください</p>
    </div>
</body>
</html>
```

### コントローラーで使用

```csharp
[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IRazorViewToStringRenderer _renderer;
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(
        IRazorViewToStringRenderer renderer,
        IInvoiceService invoiceService)
    {
        _renderer = renderer;
        _invoiceService = invoiceService;
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> DownloadInvoicePdf(int id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice == null) return NotFound();

        var viewModel = new InvoiceViewModel
        {
            InvoiceNumber = invoice.Number,
            Date = invoice.CreatedAt,
            DueDate = invoice.DueDate,
            CustomerName = invoice.Customer.Name,
            CustomerAddress = invoice.Customer.Address,
            CustomerEmail = invoice.Customer.Email,
            Items = invoice.Items.Select(i => new InvoiceItemViewModel
            {
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Total = i.Quantity * i.UnitPrice
            }).ToList(),
            Subtotal = invoice.Subtotal,
            TaxRate = invoice.TaxRate,
            TaxAmount = invoice.TaxAmount,
            Total = invoice.Total
        };

        var html = await _renderer.RenderViewToStringAsync("Pdf/InvoiceTemplate", viewModel);

        var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);

        return File(pdf.BinaryData, "application/pdf", $"invoice-{invoice.Number}.pdf");
    }
}
```

---