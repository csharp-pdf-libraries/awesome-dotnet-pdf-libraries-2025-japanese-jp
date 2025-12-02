---
**  (Japanese Translation)**

 **English:** [FAQ/mvc-view-to-pdf-csharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/mvc-view-to-pdf-csharp.md)
 **:** [FAQ/mvc-view-to-pdf-csharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/mvc-view-to-pdf-csharp.md)

---
# ASP.NET MVC および Core で Razor ビューを PDF にレンダリングする方法は？

既存の Razor ビューから ASP.NET MVC または Core プロジェクトで直接洗練されたブランドの PDF を生成する必要がありますか？IronPDF を使用すると、`.cshtml` Razor ページを高忠実度の PDF に変換できます。追加のテンプレート、HTML のコピー＆ペースト、サードパーティのサービスは不要です。この FAQ では、基本的なセットアップから高度なカスタマイズまで、知っておくべきすべてをコードファーストの回答と実用的なヒントで説明します。

---

## なぜ直接 Razor ビューを PDF にレンダリングしたいのですか？

多くのビジネスアプリケーションでは、ダウンロード可能な PDF（請求書、契約書、レポート、証明書など）が必要です。Razor マークアップを別の PDF テンプレートに手動で複製するのは面倒で、エラーが発生しやすく、しばしば一貫性が失われます。Web ページ用に使用するのと同じ Razor ビューを PDF に変換することで、コードベースを DRY（Don't Repeat Yourself）に保つことができます：

- HTML/CSS を一度設計し、ブラウザと PDF の両方で使用します。
- Razor の全機能を活用：レイアウト、パーシャル、モデルバインディングなど。
- IronPDF の Chromium レンダリングエンジンのおかげで、最新の CSS と JavaScript 機能をサポートし、ピクセルパーフェクトな出力を楽しむことができます。

Razor 以外の代替手段については、[C# で XML を PDF に変換する方法は？](xml-to-pdf-csharp.md) と [.NET MAUI で XAML を PDF にエクスポートする方法は？](xaml-to-pdf-maui-csharp.md) を参照してください。

---

## Razor ビューを PDF にレンダリングするために必要なパッケージとセットアップは？

始めるためには、IronPDF コアライブラリと、フレームワークに適した IronPDF MVC 拡張機能が必要です：

| あなたのフレームワーク      | NuGet パッケージ拡張             |
|---------------------|------------------------------------|
| ASP.NET MVC (.NET Framework)  | `IronPdf.Extensions.Mvc.Framework` |
| ASP.NET Core / .NET 5+        | `IronPdf.Extensions.Mvc.Core`      |

コアの IronPdf パッケージと関連する拡張機能の両方をインストールします：

```bash
# 従来の .NET Framework MVC の場合
Install-Package IronPdf
Install-Package IronPdf.Extensions.Mvc.Framework

# ASP.NET Core / .NET 5/6/7/8+ の場合
Install-Package IronPdf
Install-Package IronPdf.Extensions.Mvc.Core
```

[IronPDF のドキュメント](https://ironpdf.com) でセットアップガイドとトラブルシューティングのヒントを見つけることができます。

---

## コントローラーアクションで Razor ビューを PDF にレンダリングするにはどうすればいいですか？

IronPDF は Razor ビューを PDF に変換することを簡単にします。ここに、ビューをダウンロード可能な PDF としてエクスポートするコントローラーアクションの最小限の例を示します：

```csharp
using IronPdf; // NuGet 経由でインストール
using IronPdf.Extensions.Mvc.Framework; // または .NET Core の場合は .Core

public class ReportsController : Controller
{
    public ActionResult ExportMonthlyReport()
    {
        var renderer = new ChromePdfRenderer();
        var dataModel = GetMonthlyReport(); // ここでデータを取得

        var pdf = renderer.RenderView(
            HttpContext,
            "~/Views/Reports/Monthly.cshtml",
            dataModel);

        return File(pdf.BinaryData, "application/pdf", "MonthlyReport.pdf");
    }
}
```

- `ChromePdfRenderer` は、正確なレンダリングのために実際の Chromium ブラウザエンジンを使用します。
- `RenderView()` は HTTP コンテキスト、Razor ビューパス、およびモデルを受け入れます。
- 結果は PDF バイトに簡単にアクセスできる `PdfDocument` です。

プロのヒント：絶対ビューパス（`~`）と相対ビューパスの両方を使用できます。

一般的な方法で Razor ビューを PDF に変換することに興味がある場合は、詳細なガイドをチェックしてください：[C# で Razor ビューを PDF にレンダリングする方法は？](razor-view-to-pdf-csharp.md)

---

## PDF を生成する際にモデルを Razor ビューに渡すことはできますか？

もちろんです。通常の HTML レンダリングと同様に、強く型指定されたモデルを Razor ビューに渡すことができます。販売レポートの簡単な例をここに示します：

**モデルクラス：**
```csharp
public class SalesRow
{
    public string Product { get; set; }
    public int Quantity { get; set; }
    public decimal Revenue { get; set; }
}

public class SalesReport
{
    public string Title { get; set; }
    public DateTime Generated { get; set; }
    public List<SalesRow> Items { get; set; }
}
```

**コントローラーアクション：**
```csharp
using IronPdf;
using IronPdf.Extensions.Mvc.Framework;

public ActionResult DownloadSalesPdf()
{
    var model = new SalesReport
    {
        Title = "2024年6月の販売",
        Generated = DateTime.UtcNow,
        Items = salesService.GetJuneSales()
    };

    var renderer = new ChromePdfRenderer();
    var pdf = renderer.RenderView(HttpContext, "~/Views/Reports/Sales.cshtml", model);

    return File(pdf.BinaryData, "application/pdf", "Sales-June-2024.pdf");
}
```

その後、Razor ビュー（`Sales.cshtml`）は、モデルを使用して動的にテーブル、チャート、または他のコンテンツをレンダリングできます。ブラウザで動作する場合、PDF でも動作します。

より高度なテンプレートシナリオ（XML ベースを考える）については、[C# で XML を PDF に変換する方法は？](xml-to-pdf-csharp.md) を参照してください。

---

## PDF 出力で CSS を適用し、ブランドスタイリングを維持するにはどうすればよいですか？

### Razor を PDF にレンダリングする際にサポートされている CSS 機能は？

IronPDF の Chromium レンダラーは、最新の CSS の大部分—フレックスボックス、グリッド、メディアクエリ、ウェブフォントをサポートしています。最もうまく機能するのは以下の通りです：

- PDF 出力に最も信頼性の高いのは、インラインの `<style>` ブロックです。
- リンクされた外部スタイルシートはサポートされていますが、URL はサーバー環境（開発マシンだけでなく）からアクセス可能である必要があります。
- 一貫した結果を得るために、重要な CSS を Razor レイアウトまたはビューにコピーします。

**例：**
```html
<link rel="stylesheet" href="https://cdn.yourbrand.com/css/pdf.css" />
```
または、最大限の信頼性のために：
```html
<style>
    body { font-family: 'Segoe UI', Arial, sans-serif; }
    h1 { color: #1a237e; }
    /* その他のスタイル */
</style>
```

### Razor-to-PDF で共有レイアウトとパーシャルを使用できますか？

はい！Razor レイアウトとパーシャルはウェブページと全く同じように機能するため、すべての PDF でヘッダー、フッター、ブランディングを一貫して保つことができます。例えば：

```csharp
@{
    Layout = "~/Views/Shared/_PdfLayout.cshtml";
}
```

あなたの `_PdfLayout.cshtml` には `<style>` ブロック、ロゴ、ナビゲーションを含めることができます。カスタム形状や注釈を描画するヒントについては、[C# で PDF に線や四角を描画する方法は？](draw-line-rectangle-pdf-csharp.md) をチェックしてください。

### PDF エクスポートに基づいてコンテンツを表示または非表示にすることは可能ですか？

間違いなく可能です！Razor 条件文または CSS を使用して、PDF エクスポートでのみ表示される内容を制御できます：

```csharp
@if (Request.Url.AbsolutePath.Contains("Export"))
{
    <div>これはエクスポートされた PDF でのみ表示されます。</div>
}
```
または CSS で：
```css
@media print { .no-print { display: none; } }
```

---

## PDF にヘッダー、フッター、ページ番号を追加するにはどうすればよいですか？

IronPDF では、HTML ヘッダーとフッターを各ページに注入することが簡単にできます。これは、ページ番号、法的注意事項、またはブランディングを含めるのに最適です。

**カスタムヘッダーとフッターの追加例**
```csharp
using IronPdf;
using IronPdf.Extensions.Mvc.Framework;

public ActionResult ExportWithHeaders()
{
    var renderer = new ChromePdfRenderer();
    renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
    {
        HtmlFragment = "<div style='text-align:center; font-size:12px;'>機密レポート</div>",
        DrawDividerLine = true
    };
    renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
    {
        HtmlFragment = "<div style='text-align:right; font-size:10px;'>ページ {page} / {total-pages}</div>"
    };

    var pdf = renderer.RenderView(HttpContext, "~/Views/Reports/Annual.cshtml", GetReportData());
    return File(pdf.BinaryData, "application/pdf", "AnnualReport.pdf");
}
```

**利用可能なプレースホルダー：**

- `{page}`: 現在のページ番号
- `{total-pages}`: 総ページ数
- `{date}` と `{time}`: タイムスタンプ情報

ページ番号のカスタマイズについて詳しくは、[C# で PDF にページ番号を追加する方法は？](https://ironpdf.com/blog/compare-to-other-components/questpdf-add-page-number-to-pdf/) を参照してください。

---

## PDF のページサイズ、向き、余白を設定するにはどうすればよいですか？

`RenderingOptions` を使用してこれらの設定を簡単に制御できます：

```csharp
using IronPdf;
using IronPdf.Extensions.Mvc.Framework;

public ActionResult ExportLandscapeA4()
{
    var renderer = new ChromePdfRenderer();
    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
    renderer.RenderingOptions.MarginTop = 15; // ミリメートル
    renderer.RenderingOptions.MarginBottom = 15;
    renderer.RenderingOptions.MarginLeft = 10;
    renderer.RenderingOptions.MarginRight = 10;

    var pdf = renderer.RenderView(HttpContext, "~/Views/Reports/WideView.cshtml", FetchModel());
    return File(pdf.BinaryData, "application/pdf", "WideView.pdf");
}
```

DPI およびその他の高度なレンダリングオプションも設定できます。これにより、より鮮明な画像出力や QR コードが得られます。

---

## PDF をダウンロード、インラインプレビュー、またはストリーミングで提供するにはどうすればよいですか？

デフォルトでは、ファイル名を指定して `File()` 結果を返すと、ほとんどのブラウザでダウンロードがトリガーされます。PDF をブラウザタブ内でインライン表示したい場合は、`Content-Disposition` ヘッダーを調整します：

```csharp
using IronPdf;
using IronPdf.Extensions.Mvc.Framework;

public ActionResult PreviewPdf()
{
    var pdf = RenderPdfDocument();
    Response.Headers.Add("Content-Disposition", "inline; filename=Preview.pdf");
    return File(pdf.BinaryData, "application/pdf");
}
```

大きな PDF やカスタムストリーミングシナリオの場合、バイトを直接レスポンスストリームに書き込むことができます。PDF にファイル添付を含める方法については、[C# で PDF に添付ファイルを追加する方法は？](add-attachments-pdf-csharp.md) をチェックしてください。

---

## 実際の Razor-to-PDF 請求書ジェネレータはどのようなものですか？

モデル駆動型の Razor ビューから請求書 PDF を生成するための完全なエンドツーエンドの例をここに示します。

**モデル：**
```csharp
public class InvoiceLine
{
    public string Item { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total => Quantity * Price;
}

public class InvoiceModel
{
    public string InvoiceNo { get; set; }
    public string Company { get; set; }
    public DateTime DateIssued { get; set; }
    public string Customer { get; set; }
    public string Address { get; set; }
    public List<InvoiceLine> Lines { get; set; }
    public decimal Total => Lines.Sum(l => l.Total);
}
```

**コントローラー：**
```csharp
using IronPdf;
using IronPdf.Extensions.Mvc.Framework;

public class InvoiceController : Controller
{
    private readonly IInvoiceService _service;

    public InvoiceController(IInvoiceService service) => _service = service;

    public ActionResult Download(int id)
    {
        var invoice = _service.FindInvoice(id);
        if (invoice == null) return HttpNotFound();

        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.MarginTop = 20;
        renderer.RenderingOptions.MarginBottom = 20;
        renderer.RenderingOptions.HtmlFooter = new Html