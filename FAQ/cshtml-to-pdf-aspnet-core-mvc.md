# ASP.NET Core MVCでRazor（CSHTML）ビューをPDFにエクスポートする方法は？

はい、ASP.NET Core MVCのRazor（CSHTML）ビューから直接PDFファイルを生成することができます。これにより、別のHTMLテンプレートを管理したり、信頼性の低いPDFライブラリと格闘する必要がなくなります。このFAQでは、IronPDFを使用してMVC Razorビューを整形式のPDFに変換するための実用的で現代的なワークフローを、ヒント、コードサンプル、一般的な問題への実践的な解決策と共にご紹介します。

---

## なぜRazorビューをPDFテンプレートとして使用するべきなのか？

Razorビュー（CSHTMLファイル）はいくつかの理由から、優れたPDFテンプレートとなります。まず、マークアップの複製の手間が省けます。PDF専用のHTMLとウェブページの両方を管理する必要がありません。Razorを使用すると、データモデル、レイアウト、ロジックが統合され、Visual Studioでの完全なIntelliSense、型安全性、リファクタリングサポートの恩恵を受けることができます。

文字列ベースのHTMLテンプレート（Handlebars、Mustacheなど）とは異なり、RazorはMVCと密接に統合されており、部分ビュー、レイアウト、共有スタイリングを活用して一貫したブランディングを実現できます。これにより、デザイナーと開発者がウェブとPDFの出力のための単一の真実の情報源で協力することが可能になります。

PDFがMVCの外部、バックグラウンドプロセスやマイクロサービスから生成される場合は、別のレンダリングアプローチを検討するかもしれません。しかし、ウェブアプリにとって、Razorは最も保守が容易なルートです。

.NET PDFソリューションの比較については、[2025 HTMLからPDFへのソリューション：.NET比較](2025-html-to-pdf-solutions-dotnet-comparison.md)を参照してください。

---

## .NET 6/7/8でRazorビューをHTML文字列にレンダリングするにはどうすればいいですか？

RazorビューをHTML文字列に変換するには（その後PDFに変換できます）、通常のHTTPパイプラインの外でビューをレンダリングする再利用可能なサービスが必要です。以下がその方法です：

```csharp
using IronPdf; // NuGet: Install-Package IronPdf
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Threading.Tasks;

public class RazorViewToStringService
{
    private readonly ICompositeViewEngine _engine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _services;

    public RazorViewToStringService(
        ICompositeViewEngine engine,
        ITempDataProvider tempDataProvider,
        IServiceProvider services)
    {
        _engine = engine;
        _tempDataProvider = tempDataProvider;
        _services = services;
    }

    public async Task<string> RenderToStringAsync(
        ControllerContext context,
        string viewPath,
        object model)
    {
        var viewResult = _engine.FindView(context, viewPath, false);
        if (!viewResult.Success)
            throw new FileNotFoundException($"Razor view '{viewPath}' not found.");

        var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };
        var tempData = new TempDataDictionary(context.HttpContext, _tempDataProvider);

        using var writer = new StringWriter();
        var viewContext = new ViewContext(
            context,
            viewResult.View,
            viewData,
            tempData,
            writer,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return writer.ToString();
    }
}
```

**このパターンを使用する理由は？**  
- 非同期および同期のビューレンダリングの両方をサポートします。
- 任意のコントローラーやサービスに注入できます。
- レンダリングロジックをカプセル化し、テスト可能に保ちます。

通常のMVCビューを直接PDFにレンダリングする詳細については、[C#でMVCビューをPDFとして出力するにはどうすればよいですか？](mvc-view-to-pdf-csharp.md)を参照してください。

---

## RazorからHTMLをPDFファイルに変換するにはIronPDFをどのように使用しますか？

HTML文字列を取得したら、IronPDFの[ChromePdfRenderer](https://ironpdf.com/blog/videos/how-to-render-an-html-file-to-pdf-in-csharp-ironpdf/)を使用してPDFファイルを生成できます。IronPDFは積極的にメンテナンスされており、クロスプラットフォームで動作し、最新のHTMLとCSSを忠実にレンダリングします。

以下は簡単な例です：

```csharp
using IronPdf; // NuGet: Install-Package IronPdf

var pdfRenderer = new ChromePdfRenderer();
var pdfDocument = pdfRenderer.RenderHtmlAsPdf(htmlContent);
pdfDocument.SaveAs("invoice.pdf"); // pdfDocument.BinaryDataにアクセスしてストリームやバイト配列を返すこともできます
```

外部プロセスや複雑なセットアップは不要です。HTMLを渡して高品質のPDFを取得するだけです。

IronPDFが他のPDFツールとどのように比較されるかについては、[2025 HTMLからPDFへのソリューション：.NET比較](2025-html-to-pdf-solutions-dotnet-comparison.md)を参照してください。

---

## MVCコントローラーからPDF請求書をダウンロードする完全な例を示してもらえますか？

もちろんです！以下は、ユーザーがPDFとして請求書をダウンロードできるコントローラーの例です。上記のサービスを使用してRazorビューをレンダリングし、IronPDFでPDFを生成してストリームします：

```csharp
using IronPdf;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class InvoicesController : Controller
{
    private readonly RazorViewToStringService _viewService;

    public InvoicesController(RazorViewToStringService viewService)
    {
        _viewService = viewService;
    }

    [HttpGet("invoices/{id}/pdf")]
    public async Task<IActionResult> GetInvoicePdf(int id)
    {
        var invoiceModel = await FetchInvoiceViewModelAsync(id);

        var html = await _viewService.RenderToStringAsync(
            ControllerContext,
            "Invoices/PdfInvoiceTemplate",
            invoiceModel);

        var pdfRenderer = new ChromePdfRenderer();
        var pdf = pdfRenderer.RenderHtmlAsPdf(html);

        return File(pdf.BinaryData, "application/pdf", $"invoice-{id}.pdf");
    }

    private async Task<InvoicePdfViewModel> FetchInvoiceViewModelAsync(int id)
    {
        // データアクセスを置き換えてください
        return new InvoicePdfViewModel
        {
            InvoiceNumber = $"INV-{id:0000}",
            CustomerName = "Acme Corp",
            CustomerAddress = "123 Main St",
            InvoiceDate = DateTime.UtcNow,
            LineItems = new List<LineItemViewModel>
            {
                new LineItemViewModel { Description = "Consulting", Quantity = 8, Price = 120 },
                new LineItemViewModel { Description = "Support", Quantity = 4, Price = 80 }
            },
            Subtotal = 1280,
            Tax = 128,
            Total = 1408,
            PaymentTerms = "30日以内に支払い"
        };
    }
}
```

このパターンを使用すると、一時ファイルやサブプロセスを扱う必要がなく、すべてC#で完結します。

PDFをストリームやメモリとして返す方法についての詳細は、[C#でPDF MemoryStreamsを扱う方法は？](pdf-memorystream-csharp.md)を参照してください。

---

### 典型的なRazor PDFビューはどのようなものですか？

PDFエクスポート用のRazorビューは、好みに応じてシンプルにも詳細にもできます。以下は、`/Views/Invoices/PdfInvoiceTemplate.cshtml`に配置するかもしれないクリーンな例です：

```html
@model InvoicePdfViewModel
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Invoice @Model.InvoiceNumber</title>
    <style>
        @page { size: A4; margin: 20mm; }
        body { font-family: Arial, sans-serif; color: #333; }
        .header { font-size: 22px; font-weight: bold; margin-bottom: 16px; }
        .address { margin-bottom: 18px; }
        table { width: 100%; border-collapse: collapse; }
        th, td { border-bottom: 1px solid #ccc; padding: 7px; }
        th { background: #f0f0f0; }
        .totals { text-align: right; font-weight: bold; }
    </style>
</head>
<body>
    <div class="header">Invoice @Model.InvoiceNumber</div>
    <div class="address">
        <strong>To:</strong> @Model.CustomerName<br />
        @Model.CustomerAddress<br />
        <strong>Date:</strong> @Model.InvoiceDate.ToShortDateString()
    </div>
    <table>
        <thead>
            <tr>
                <th>Description</th>
                <th>Qty</th>
                <th>Price</th>
                <th>Amount</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var item in Model.LineItems)
            {
                <tr>
                    <td>@item.Description</td>
                    <td>@item.Quantity</td>
                    <td>@item.Price.ToString("C")</td>
                    <td>@(item.Quantity * item.Price).ToString("C")</td>
                </tr>
            }
        </tbody>
    </table>
    <div class="totals">
        Subtotal: @Model.Subtotal.ToString("C")<br />
        Tax: @Model.Tax.ToString("C")<br />
        <span>Total: @Model.Total.ToString("C")</span>
    </div>
    <div>Payment Terms: @Model.PaymentTerms</div>
</body>
</html>
```

**ヒント：** PDFレンダリングでは、外部CSSよりもインラインスタイルがはるかに信頼性が高いです。

---

## PDFでRazorレイアウト、パーシャル、共有スタイルをどのように使用しますか？

Razorのレイアウトとパーシャルビューのサポートにより、異なるPDF間でヘッダー、フッター、複雑なセクションを再利用できます。ウェブページと同様です。

### すべてのPDFに一貫したヘッダーとフッターを追加するにはどうすればよいですか？

`/Views/Shared/_PdfLayout.cshtml`にレイアウトを作成します：

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <style>
        body { margin: 28px; font-family: Verdana, sans-serif; }
        .pdf-header { text-align: center; border-bottom: 2px solid #333; margin-bottom: 12px; }
        .pdf-footer { text-align: center; color: #555; font-size: 11px; border-top: 1px solid #bbb; margin-top: 24px; }
    </style>
</head>
<body>
    <div class="pdf-header">
        <img src="https://placehold.co/150x50?text=Logo" alt="Logo" /><br />
        <span>Your Company Name – Invoices</span>
    </div>
    @RenderBody()
    <div class="pdf-footer">
        Generated: @DateTime.Now.ToShortDateString()
    </div>
</body>
</html>
```

PDFテンプレートでレイアウトを指定します：

```csharp
@{
    Layout = "_PdfLayout";
}
```

### 複数のPDFでテーブルセクションを再利用するにはどうすればよいですか？

`/Views/Shared/_LineItemsTable.cshtml`のようなパーシャルビューを定義し、PDFで使用します：

```html
@model List<LineItemViewModel>
<table>
    <thead>
        <tr>
            <th>Description</th><th>Qty</th><th>Price</th><th>Amount</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model)
        {
            <tr>
                <td>@item.Description</td>
                <td>@item.Quantity</td>
                <td>@item.Price.ToString("C")</td>
                <td>@(item.Quantity * item.Price).ToString("C")</td>
            </tr>
        }
    </tbody>
</table>
```

参照するには：

```csharp
@await Html.PartialAsync("_LineItemsTable", Model.LineItems)
```

PDFに画像、線、または長方形を追加したり、描画したりする方法についての詳細は、[C#でPDFに画像を追加する方法は？](add-images-to-pdf-csharp.md)および[C#でPDFに線や長方形を描画する方法は？](draw-line-rectangle-pdf-csharp.md)を参照してください。

---

## 複数ページのPDFでページブレーク、ヘッダー、フッターをどのように扱いますか？

### Razor PDF出力でページブレークを制御するにはどうすればよいですか？

印刷用のCSSを使用してページブレークを管理します：

```css
.section { page-break-after: always; }
.no-break { page-break-inside: avoid; }
thead { display: table-header-group; }
tfoot { display: table-footer-group; }
```

- `page-break-after: always;`は、特定のセクションの後に強制的なページブレークを追加します。
- `page-break-inside: avoid;`は、テーブルやブロックが分割されるのを防ぎます。
- テーブルヘッダー/フッターグループは、ヘッダーが各ページに繰り返されることを保証します。

詳細については、公式の[HTMLからPDFへのページブレークガイド](https://ironpdf.com/how-to/html-to-pdf-page-breaks/)を参照してください。

### 動的なページ番号をどのように追加しますか？

IronPDFを使用すると、フッターオプションを介してページ番号を挿入できます：

```csharp
var renderer = new ChromePdfRenderer();
renderer.PrintOptions.Footer = new SimpleHeaderFooter()
{
    CenterText = "Page {page} of {total-pages}",
    DrawDividerLine = true
};
```

これにより、プレースホルダーが正しい値で自動的に置き換えられます。

C#でPDFにページ番号を追加する方法についても参照して