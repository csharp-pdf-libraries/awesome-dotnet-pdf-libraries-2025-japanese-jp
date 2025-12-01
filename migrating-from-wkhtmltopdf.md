---
**🌐 日本語版** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/migrating-from-wkhtmltopdf.md)

---

# wkhtmltopdfからの移行：現代の代替品への完全ガイド

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFのクリエーター

[![最終更新](https://img.shields.io/badge/Updated-November%202025-green)]()
[![セキュリティ](https://img.shields.io/badge/CVE--2022--35583-CRITICAL-red)]()

> wkhtmltopdfは放棄され、未修正の重大な脆弱性があり、現代のCSSをレンダリングできません。まだそれ（またはDinkToPdf、NReco、Rotativa、TuesPechkinを）使用している場合は、移行する時が来ました。ここに方法があります。

---

## 目次

1. [移行する必要がある理由](#移行する必要がある理由)
2. [影響を受けるライブラリ](#影響を受けるライブラリ)
3. [移行オプション](#移行オプション)
4. [IronPDFへの段階的移行](#ironpdfへの段階的移行)
5. [APIマッピング](#apiマッピング)
6. [コード例](#コード例)
7. [一般的な問題と解決策](#一般的な問題と解決策)

---

## 移行する必要がある理由

### 重大なセキュリティ脆弱性

**CVE-2022-35583** (CVSS 9.8 - CRITICAL)

wkhtmltopdfはサーバーサイドリクエストフォージェリ（SSRF）に対して脆弱です。攻撃者は、wkhtmltopdfに以下をさせるHTMLを作成できます：

- 内部ネットワークリソースへのアクセス
- ローカルファイルの読み取り
- インフラストラクチャのプローブ
- 潜在的にリモートコード実行の達成

**この脆弱性は未修正であり、wkhtmltopdfが放棄されたため、決して修正されることはありません。**

### プロジェクトの放棄

| 指標 | 状態 |
|-----------|--------|
| 最後の意味のあるコミット | 2020 |
| 未解決の問題 | 1,500+ 未対応 |
| プルリクエスト | 無視された |
| メンテナーの反応 | なし |
| セキュリティパッチ | なし |

### 古いレンダリングエンジン

wkhtmltopdfは2015年のQt WebKitを使用しています：

| 機能 | wkhtmltopdf | 現代のブラウザ |
|---------|-------------|-----------------|
| CSS Flexbox | ❌ 壊れている | ✅ 完全サポート |
| CSS Grid | ❌ なし | ✅ 完全サポート |
| ES6+ JavaScript | ❌ なし | ✅ 完全サポート |
| CSS変数 | ❌ なし | ✅ 完全サポート |
| Webフォント | ⚠️ 部分的 | ✅ 完全サポート |

**あなたのHTMLが現代のCSSを使用している場合、wkhtmltopdfは壊れた出力を生成します。**

---

## 影響を受けるライブラリ

これらのいずれかを使用している場合、影響を受けます：

| ライブラリ | それは何か | CVEを継承 |
|---------|-----------|--------------|
| **wkhtmltopdf** | 基本ツール | ✅ はい |
| **DinkToPdf** | .NET Coreラッパー | ✅ はい |
| **NReco.PdfGenerator** | .NETラッパー | ✅ はい |
| **Rotativa** | ASP.NET MVCラッパー | ✅ はい |
| **TuesPechkin** | スレッドセーフラッパー | ✅ はい |
| **Haukcode.DinkToPdf** | DinkToPdfのフォーク | ✅ はい |

これらのライブラリはすべてwkhtmltopdfのラッパーです。それらはすべてのセキュリティ脆弱性とレンダリングの制限を継承します。

---

## 移行オプション

### オプション 1: IronPDF（推奨）

**長所:**
- ✅ フルChromiumレンダリング（現代のCSS）
- ✅ 組み込みPDF操作
- ✅ シンプルなAPI移行
- ✅ プロフェッショナルサポート
- ✅ アクセシビリティ準拠

**短所:**
- 商用ライセンス（$749）

### オプション 2: PuppeteerSharp

**長所:**
- ✅ フルChromiumレンダリング
- ✅ 無料（Apache 2.0）

**短所:**
- ❌ 300MBのデプロイメント（Chromiumをダウンロード）
- ❌ PDF操作なし
- ❌ メモリ管理が必要
- ❌ より複雑なAPI

### オプション 3: Playwright

**長所:**
- ✅ フルChromiumレンダリング
- ✅ 無料（Apache 2.0）
- ✅ Microsoft支援

**短所:**
- ❌ 400MB+のデプロイメント（3つのブラウザ）
- ❌ テストに焦点（PDFは二次的）
- ❌ PDF操作なし

### 推奨

ほとんどのチームにとって、**IronPDF**は最もスムーズな移行パスを提供し、最もシンプルなAPIと完全な機能セットを備えています。ライセンスコストは、節約された開発者時間で回収されます。

---

## IronPDFへの段階的移行

### ステップ 1: 古いパッケージの削除

```bash
# wkhtmltopdfラッパーを削除
dotnet remove package DinkToPdf
dotnet remove package NReco.PdfGenerator
dotnet remove package Rotativa.AspNetCore
dotnet remove package TuesPechkin

# プロジェクトからwkhtmltopdfバイナリを削除
rm -rf wkhtmltopdf/
rm -rf libwkhtmltox*
```

### ステップ 2: IronPDFのインストール

```bash
dotnet add package IronPdf
```

### ステップ 3: コードの更新

以下のAPIマッピングと例を参照してください。

### ステップ 4: レンダリングのテスト

```csharp
// 実際のHTMLでテスト
var pdf = ChromePdfRenderer.RenderHtmlAsPdf(yourHtml);
pdf.SaveAs("test-output.pdf");

// wkhtmltopdfの出力と比較 - IronPDFはブラウザのレンダリングと一致するはずです
```

### ステップ 5: ネイティブ依存関係の削除

以下からwkhtmltopdfバイナリを削除します：
- プロジェクトフォルダ
- Dockerイメージ
- CI/CDパイプライン
- サーバーデプロイメント

---

## APIマッピング

### DinkToPdfからIronPDFへ

| DinkToPdf | IronPDF |
|-----------|---------|
| `new SynchronizedConverter(new PdfTools())` | `new ChromePdfRenderer()` |
| `converter.Convert(document)` | `renderer.RenderHtmlAsPdf(html)` |
| `GlobalSettings.DocumentTitle` | `renderer.RenderingOptions.Title` |
| `GlobalSettings.PaperSize` | `renderer.RenderingOptions.PaperSize` |
| `GlobalSettings.Margins` | `renderer.RenderingOptions.MarginTop/Bottom/Left/Right` |
| `ObjectSettings.HtmlContent` | `RenderHtmlAsPdf()`への最初のパラメータ |
| `ObjectSettings.WebSettings.DefaultEncoding` | デフォルトでUTF-8 |

### RotativaからIronPDFへ

| Rotativa | IronPDF |
|----------|---------|
| `new ViewAsPdf("ViewName", model)` | ビューをHTMLにレンダリングし、その後`RenderHtmlAsPdf()` |
| `new UrlAsPdf("https://...")` | `ChromePdfRenderer.RenderUrlAsPdf()` |
| `CustomSwitches = "--page-size A4"` | `renderer.RenderingOptions.PaperSize = PdfPaperSize.A4` |
| `PageMargins` | `renderer.RenderingOptions.MarginTop/Bottom/Left/Right` |

### NRecoからIronPDFへ

| NReco | IronPDF |
|-------|---------|
| `new HtmlToPdfConverter()` | `new ChromePdfRenderer()` |
| `converter.GeneratePdf(html)` | `renderer.RenderHtmlAsPdf(html).BinaryData` |
| `converter.GeneratePdfFromFile(path)` | `renderer.RenderHtmlFileAsPdf(path)` |
| `converter.PageWidth` | `renderer.RenderingOptions.PaperSize` |
| `converter.Margins` | `renderer.RenderingOptions.MarginTop/Bottom/Left/Right` |

---

## コード例

### DinkToPdfの前：

```csharp
// 古い: DinkToPdf
using DinkToPdf;
using DinkToPdf.Contracts;

public class PdfService
{
    private readonly IConverter _converter;

    public PdfService()
    {
        _converter = new SynchronizedConverter(new PdfTools());
    }

    public byte[] GeneratePdf(string html)
    {
        var doc = new HtmlToPdfDocument
        {
            GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10 }
            },
            Objects = {
                new ObjectSettings {
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
        };

        return _converter.Convert(doc);
    }
}
```

### IronPDFの後：

```csharp
// 新しい: IronPDF
using IronPdf;

public class PdfService
{
    private readonly ChromePdfRenderer _renderer;

    public PdfService()
    {
        _renderer = new ChromePdfRenderer();
        _renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        _renderer.RenderingOptions.MarginTop = 10;
        _renderer.RenderingOptions.MarginBottom = 10;
    }

    public byte[] GeneratePdf(string html)
    {
        return _renderer.RenderHtmlAsPdf(html).BinaryData;
    }
}
```

### Rotativaの前：

```csharp
// 古い: Rotativa
using Rotativa.AspNetCore;

public class ReportsController : Controller
{
    public IActionResult Invoice(int id)
    {
        var model = _invoiceService.GetById(id);
        return new ViewAsPdf("InvoiceView", model)
        {
            PageSize = Size.A4,
            PageMargins = new Margins(10, 10, 10, 10),
            CustomSwitches = "--print-media-type"
        };
    }
}
```

### IronPDFの後：

```csharp
// 新しい: IronPDF
using IronPdf;

public class ReportsController : Controller
{
    private readonly IRazorViewToStringRenderer _viewRenderer;

    public async Task<IActionResult> Invoice(int id)
    {
        var model = _invoiceService.GetById(id);
        var html = await _viewRenderer.RenderViewToStringAsync("InvoiceView", model);

        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        renderer.RenderingOptions.MarginTop = 10;
        renderer.RenderingOptions.MarginBottom = 10;
        renderer.RenderingOptions.CssMediaType = PdfCssMediaType.Print;

        var pdf = renderer.RenderHtmlAsPdf(html);
        return File(pdf.BinaryData, "application/pdf", $"invoice-{id}.pdf");
    }
}
```

### NRecoの前：

```csharp
// 古い: NReco
using NReco.PdfGenerator;

public byte[] ConvertHtmlToPdf(string html)
{
    var converter = new HtmlToPdfConverter();
    converter.PageWidth = 210;
    converter.PageHeight = 297;
    converter.Margins = new PageMargins { Top = 10, Bottom = 10 };

    return converter.GeneratePdf(html);
}
```

### IronPDFの後：

```csharp
// 新しい: IronPDF
using IronPdf;

public byte[] ConvertHtmlToPdf(string html)
{
    var renderer = new ChromePdfRenderer();
    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    renderer.RenderingOptions.MarginTop = 10;
   /renderer.RenderingOptions.MarginBottom = 10;

    return renderer.RenderHtmlAsPdf(html).BinaryData;
}
```

---

## 一般的な問題と解決策

### 問題 1: Flexbox/Gridがレンダリングされるようになった

**症状:** PDFが以前とは異なり（より良く）見える

**理由:** wkhtmltopdfは現代のCSSをレンダリングできませんでした。IronPDFは正しくレンダリングします。

**解決策:** これは予想されることです。あなたのPDFは今、ユーザーがブラウザで見るものと一致します。

### 問題 2: JavaScriptが実行される

**症状:** 以前には表示されなかった動的コンテンツが表示されます

**理由:** wkhtmltopdfは限定的なJavaScriptしか持っていませんでした。IronPDFは完全なJavaScriptを実行します。

**解決策:** 通常は有益です。問題がある場合は、以下で無効にします：

```csharp
renderer.RenderingOptions.EnableJavaScript = false;
```

### 問題 3: Webフォントが読み込まれる

**症状:** フォントが異なって見える（通常はより良い）

**理由:** wkhtmltopdfはWebフォントのサポートが貧弱でした。

**解決策:** 通常は有益です。一貫性のために、HTMLがWebフォントを使用していることを確認してください：

```html
<link href="https://fonts.googleapis.com/css2?family=Roboto" rel="stylesheet">
```

### 問題 4: カスタムwkhtmltopdfスイッチ

**症状:** カスタムコマンドラインスイッチがもう機能しない

**解決策:** IronPDFの同等物にマップします：

| wkhtmltopdfスイッチ | IronPDF同等物 |
|-------------------|-------------------|
| `--page-size A4` | `PaperSize = PdfPaperSize.A4` |
| `--orientation Landscape` | `PaperOrientation = PdfPaperOrientation.Landscape` |
| `--margin-top 10` | `MarginTop = 10` |
| `--print-media-type` | `CssMediaType = PdfCssMediaType.Print` |
| `--javascript-delay 200` | `WaitFor.JavaScript(200)` |
| `--disable-javascript` | `EnableJavaScript = false` |
| `--header-html file.html` | `HtmlHeader = new HtmlHeaderFooter { ... }` |

### 問題 5: ネイティブライブラリのエラー

**症状:** libwkhtmltoxに関するエラーが発生します

**解決策:** すべてのwkhtmltopdfネイティブ依存関係を削除します：

```bash
# Linux
rm -rf /usr/local/lib/libwkhtmlto