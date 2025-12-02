# C#でRazorビューをPDFにレンダリングする方法は？

C#でRazorビューを直接PDFにレンダリングすることは、請求書、レポートなどの印刷可能なドキュメント用に既存のUIテンプレートを活用する強力な方法です。IronPDFを使用すると、モデル、レイアウト、パーシャル、および高度なロジックを含む`.cshtml` Razorファイルを高忠実度のPDFに変換できます。これは、テンプレートの複製や新しい構文の学習なしに行えます。このFAQでは、セットアップからトラブルシューティングまでのプロセスを、実用的なコードサンプルと実際のシナリオに対する洞察とともに説明します。

---

## C#でRazorビューをPDFにすばやくレンダリングするには？

すぐに始めたい場合は、IronPDFとそのRazor拡張機能が必要です。RazorビューをPDFにレンダリングする最も簡単な方法は次のとおりです：

```csharp
using IronPdf;
using IronPdf.Extensions.Razor;
// Install-Package IronPdf
// Install-Package IronPdf.Extensions.Razor

var pdfGenerator = new ChromePdfRenderer();
var pdfDoc = pdfGenerator.RenderRazorToPdf("/Views/Invoice.cshtml", model);
pdfDoc.SaveAs("invoice.pdf");
```
わずか数行で、任意のRazorビューをPDFに変換できます。すでにMVCやRazorページでRazorを使用している場合、ワークフローは非常に馴染み深いものに感じられるでしょう。

---

## HTML文字列や他のテンプレートの代わりにPDF生成にRazorビューを使用する理由は？

### RazorビューをPDFに使用する主な利点は何ですか？

RazorをPDFに使用すると、以下のことができます：

- **既存のフロントエンドテンプレートを再利用する：** Web用とPDF用に別々のHTMLを維持する必要はありません。
- **強い型付けを活用する：** コンパイル時の安全性とIntelliSenseのためにC#モデルを使用します。
- **レイアウトとパーシャルを使用する：** `_Layout.cshtml`やパーシャルビューでブランディングと構造を維持します。
- **実際のC#ロジックを書く：** 不格好または馴染みのないテンプレート構文はありません。完全な言語サポートがあります。
- **ブラウザでプレビューする：** 最初に通常のWebページとしてPDFのデザイン、スタイル、デバッグを行います。

PDF生成にRazorを選択する理由についてさらに詳しく知りたい場合は、IronPDFの[Razorページのドキュメント](https://ironpdf.com/docs/questions/razor-pdf/)をご覧ください。

---

## 必要なパッケージは何で、プロジェクトはどのように構成されるべきですか？

### どのNuGetパッケージをインストールする必要がありますか？

RazorからPDFへのレンダリングを有効にするには、次のNuGetパッケージをインストールします：

```powershell
Install-Package IronPdf
Install-Package IronPdf.Extensions.Razor
```
または、.NET CLIの場合：

```bash
dotnet add package IronPdf
dotnet add package IronPdf.Extensions.Razor
```

### Razor PDFテンプレートはどこに配置するべきですか？

- PDF関連のRazorファイルを`/Views`フォルダ（または好みのRazorディレクトリ）に保持します。
- 明確さと安全性のために強いモデルを使用します。
- PDFレンダラーがアクセスできるように、静的アセット（画像やCSSなど）が利用可能であることを確認してください。画像のヒントについては、[Razor-PDFワークフローで画像を扱う方法は？](#how-do-i-handle-images-in-razor-pdf-workflows)を参照してください。

---

## 強く型指定されたRazorビューからPDFを生成するにはどうすればよいですか？

### Razor-to-PDFでMVCモデルを使用できますか？

もちろんです！Web用に設計されたRazorビューと同様に、モデルをレンダラーに直接渡します。たとえば、`InvoiceModel`があるとします：

```csharp
public class InvoiceModel
{
    public string InvoiceNumber { get; set; }
    public DateTime Date { get; set; }
    public string CustomerName { get; set; }
    public List<LineItem> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);
}

public class LineItem
{
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
```

あなたのRazorビュー（`Views/Invoice.cshtml`）は、MVCと同じようにこのモデルを使用できます。

### PDFをレンダリングして保存するにはどうすればよいですか？

```csharp
using IronPdf;
using IronPdf.Extensions.Razor;

var invoice = new InvoiceModel
{
    InvoiceNumber = "INV-2024-001",
    Date = DateTime.UtcNow,
    CustomerName = "Contoso Ltd.",
    Items = new List<LineItem>
    {
        new() { Description = "Service Fee", Quantity = 2, UnitPrice = 150.00m },
        new() { Description = "Consulting", Quantity = 5, UnitPrice = 75.00m }
    }
};

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderRazorToPdf("/Views/Invoice.cshtml", invoice);
pdf.SaveAs("output-invoice.pdf");
```

PDFを生成する前にブラウザで`.cshtml`をプレビューして、視覚的な微調整をすばやく行うことができます。

---

## 一貫したPDFブランディングのためにレイアウトとセクションをどのように使用しますか？

### PDFテンプレートにRazorレイアウトを使用できますか？

はい！Razorレイアウト（`_Layout.cshtml`やカスタムレイアウトなど）を使用すると、一貫したヘッダー、フッター、および全体的な構造を維持できます。たとえば、レイアウトにはロゴ、連絡先の詳細、または標準化されたフッターが含まれる場合があります。

#### レイアウトの例（`Views/Shared/_PdfLayout.cshtml`）：

```html
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: 'Segoe UI', sans-serif; margin: 0; padding: 40px; }
        header { border-bottom: 2px solid #333; padding-bottom: 15px; margin-bottom: 25px; }
        footer { position: fixed; bottom: 10px; width: 90%; text-align: center; color: #888; }
    </style>
    @RenderSection("Styles", required: false)
</head>
<body>
    <header>
        <img src="https://example.com/logo.png" height="45" alt="Logo"/>
    </header>
    @RenderBody()
    <footer>
        &copy; @DateTime.Now.Year My Company | Confidential
    </footer>
</body>
</html>
```

#### ビューでの使用例：

```html
@model ReportModel
@{
    Layout = "_PdfLayout";
}

@section Styles {
    <style>
        .report-table { border-collapse: collapse; width: 100%; }
    </style>
}

<h1>@Model.Title</h1>
<p>Generated: @DateTime.Now</p>
```

**レイアウトを使用する理由は？** ブランディングが変更された場合、ファイルを1つ更新するだけです。

---

## PDFでコンポーネントを再利用するには、パーシャルビューをどのように使用しますか？

### Razor-PDFワークフローでパーシャルビューはどのように機能しますか？

複雑なドキュメントを管理しやすく、再利用可能な部分に分割します。たとえば、`_CustomerInfo.cshtml`や`_LineItem.cshtml`をパーシャルとして持つことができます。

#### パーシャルの例（`Views/Shared/_CustomerInfo.cshtml`）：

```html
@model Customer
<div>
    <strong>@Model.Name</strong><br/>
    @Model.Address<br/>
    @Model.Email
</div>
```

#### メインテンプレートでのパーシャルビューの使用：

```html
@model OrderModel

<partial name="_CustomerInfo" model="Model.Customer" />

@foreach (var item in Model.Items)
{
    <partial name="_LineItem" model="item" />
}
```

これにより、テンプレートをDRY（Don't Repeat Yourself）に保ち、メンテナンスが容易になります。

---

## Razor-to-PDFでViewDataとViewBagを使用できますか？

### モデルを超えて動的データを渡すにはどうすればよいですか？

IronPDFのRazor拡張機能は、通常のMVCと同様に`ViewData`と`ViewBag`をサポートしています。これは、モデルと一緒に構成、ランタイム設定、またはブランディング情報を渡すのに便利です。

#### 例：

```csharp
using IronPdf;
using IronPdf.Extensions.Razor;

var renderer = new ChromePdfRenderer();
var extraData = new Dictionary<string, object>
{
    { "BrandName", "Acme Inc." },
    { "ReportDate", DateTime.Today }
};

var pdf = renderer.RenderRazorToPdf("/Views/Report.cshtml", model, extraData);
```

そして、Razorテンプレートで：

```html
@{
    var brand = ViewData["BrandName"];
    var date = ViewData["ReportDate"];
}
<h1>@brand Report</h1>
<p>Date: @date</p>
```

このアプローチは、主要なモデルの一部ではない値を注入するのに特に便利です。

---

## 条件ロジックとフォーマットに完全なC#をサポートしていますか？

### PDF内でC#ロジックとデータフォーマットをどのように使用しますか？

Razor構文は完全なC#ロジックをサポートしているため、`if`、`switch`、ループなどを使用できます：

```html
@if (Model.IsPaid)
{
    <div class="status-paid">PAID</div>
}
else
{
    <div class="status-unpaid">Due by @Model.DueDate.ToShortDateString()</div>
}

@switch (Model.Status)
{
    case OrderStatus.Pending:
        <span class="badge badge-warning">Pending</span>
        break;
    case OrderStatus.Shipped:
        <span class="badge badge-info">Shipped</span>
        break;
    case OrderStatus.Completed:
        <span class="badge badge-success">Completed</span>
        break;
}
```

### データにカスタムフォーマットを適用するにはどうすればよいですか？

日付、通貨などをC#を使用してフォーマットできます：

```html
@foreach (var entry in Model.Entries)
{
    <tr>
        <td>@entry.Date.ToString("dd MMM yyyy")</td>
        <td>@entry.Amount.ToString("C")</td>
        <td class="@(entry.Amount < 0 ? "negative" : "positive")">
            @entry.Amount
        </td>
    </tr>
}
```

**ヒント：** RazorはWebビューと同じようにレンダリングされるため、PDFにレンダリングする前にブラウザでプレビューして、フォーマットの問題をキャッチします。

---

## ASP.NETコントローラーからダウンロード用のPDFを生成するにはどうすればよいですか？

### WebアプリからユーザーにPDFをダウンロードさせるにはどうすればよいですか？

コントローラーからオンデマンドでPDFを生成することは簡単です。ASP.NET CoreでPDFをファイルダウンロードとして返す方法は次のとおりです：

```csharp
using IronPdf;
using IronPdf.Extensions.Razor;
using Microsoft.AspNetCore.Mvc;

public class ReportsController : Controller
{
    public IActionResult DownloadReport(int id)
    {
        var reportData = _reportService.GetReport(id);

        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderRazorToPdf("/Views/Report.cshtml", reportData);

        return File(pdf.BinaryData, "application/pdf", $"report-{id}.pdf");
    }
}
```

### PDFを非同期で生成できますか？

はい—IronPDFは、大規模または複雑なドキュメントをレンダリングするための非同期メソッドをサポートしています：

```csharp
var pdf = await renderer.RenderRazorToPdfAsync("/Views/Report.cshtml", reportData);
```

MVCとの統合については、[C#でMVCビューからPDFを生成する方法は？](mvc-view-to-pdf-csharp.md)を参照してください。

---

## PDFにページ番号、ヘッダー、フッターを追加するにはどうすればよいですか？

### ページ番号を挿入する最良の方法は何ですか？

IronPDFの`HtmlHeaderFooter`機能を使用して、ページ番号やカスタムヘッダー、フッターを追加できます：

```csharp
using IronPdf;
using IronPdf.Extensions.Razor;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = "<div style='text-align:center;'>Page {page} of {total-pages}</div>",
    DrawDividerLine = true
};
var pdf = renderer.RenderRazorToPdf("/Views/Report.cshtml", model);
pdf.SaveAs("paged-report.pdf");
```

サポートされているトークンには、現在のページに対して`{page}`、ページの合計数に対して`{total-pages}`が含まれます。

高度なページ分割については、[PDFページ分割をどのように制御しますか？](https://ironpdf.com/how-to/html-to-pdf-page-breaks/)を参照してください。

---

## Razor-PDFワークフローで画像を扱うにはどうすればよいですか？

### PDFに画像を含める最良の方法は何ですか？

画像は次の方法でリンクできます：

- **絶対URL：**  
  ```html
  <img src="https://mydomain.com/images/logo.png" />
  ```
- **Base64エ