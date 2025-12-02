# wkhtmltopdfからIronPDFへの移行方法は？

## wkhtmltopdfからIronPDFへの移行理由

**wkhtmltopdfは重大なセキュリティリスクです。** このプロジェクトには、サーバーサイドリクエストフォージェリ（SSRF）を許可し、攻撃者がインフラを乗っ取る可能性がある重大な脆弱性（CVE-2022-35583、CVSS 9.8）があります。この脆弱性は**決して修正されることはありません**。なぜなら、このプロジェクトは2016-2017年に**正式に放棄された**からです。

### セキュリティ危機

| 問題 | 重大度 | 状態 |
|-------|----------|--------|
| **CVE-2022-35583** | CRITICAL (9.8/10) | **未修正** |
| **SSRF脆弱性** | インフラ乗っ取りリスク | **未修正** |
| **最終更新** | 2016-2017 | **放棄された** |
| **WebKitバージョン** | 2015 (Qt WebKit) | **時代遅れ** |
| **CSSグリッドサポート** | なし | 壊れている |
| **Flexboxサポート** | 部分的 | 壊れている |
| **ES6+ JavaScript** | なし | 壊れている |

**wkhtmltopdfを使用し続けることで、インフラが危険にさらされています。**

---

## 放棄問題

wkhtmltopdfは単に時代遅れなだけでなく、将来性のない死んだプロジェクトです：

| 項目 | wkhtmltopdf | IronPDF |
|--------|-------------|---------|
| **セキュリティ状態** | CRITICAL CVE 未修正 | 既知のCVEなし |
| **最後の有意義な更新** | 2016-2017 | アクティブな開発 |
| **レンダリングエンジン** | Qt WebKit (2015) | 現代的なChromium |
| **CSSグリッド** | ❌ サポートされていない | ✅ 完全サポート |
| **Flexbox** | ⚠️ 壊れている | ✅ 完全サポート |
| **ES6+ JavaScript** | ❌ サポートされていない | ✅ 完全サポート |
| **Async/Await** | ❌ サポートされていない | ✅ 完全サポート |
| **PDF操作** | ❌ サポートされていない | ✅ 完全サポート |
| **デジタル署名** | ❌ サポートされていない | ✅ 完全サポート |
| **PDF/A準拠** | ❌ サポートされていない | ✅ 完全サポート |
| **プロフェッショナルサポート** | なし（放棄された） | 商用SLA付き |

### 影響を受けるラッパーライブラリ

wkhtmltopdfのすべての.NETラッパーは、これらの脆弱性を受け継ぎます：

| ラッパーライブラリ | 状態 | セキュリティリスク |
|-----------------|--------|---------------|
| **DinkToPdf** | 放棄された | ⚠️ CRITICAL |
| **Rotativa** | 放棄された | ⚠️ CRITICAL |
| **TuesPechkin** | 放棄された | ⚠️ CRITICAL |
| **WkHtmlToPdf-DotNet** | 放棄された | ⚠️ CRITICAL |
| **NReco.PdfGenerator** | wkhtmltopdfを使用 | ⚠️ CRITICAL |

**これらのライブラリを使用している場合、CVE-2022-35583に対して脆弱です。**

---

## CVE-2022-35583（SSRF）の理解

サーバーサイドリクエストフォージェリ脆弱性は、攻撃者が以下を可能にします：

1. **内部サービスへのアクセス**：ファイアウォールの後ろにある内部API、データベース、サービスに到達
2. **認証情報の盗難**：IAM認証情報を盗むために、クラウドメタデータエンドポイント（AWS、GCP、Azure）にアクセス
3. **ポートスキャン**：インフラ内部から内部ネットワークをスキャン
4. **データの抽出**：作成されたHTML/CSSを通じて機密データを抽出

### 攻撃の仕組み

```html
<!-- PDFジェネレータに提出された悪意のあるHTML -->
<iframe src="http://169.254.169.254/latest/meta-data/iam/security-credentials/"></iframe>
<img src="http://internal-database:5432/admin"/>
```

wkhtmltopdfがこのHTMLをレンダリングすると、サーバーのネットワークコンテキストからこれらのURLをフェッチし、ファイアウォールとセキュリティコントロールをバイパスします。

**IronPDFは、安全なデフォルト設定とネットワーク分離オプションでこれを軽減します。**

---

## NuGetパッケージの変更

```bash
# 使用しているwkhtmltopdfラッパーを削除（どれを使用しているかによる）
dotnet remove package WkHtmlToPdf-DotNet
dotnet remove package DinkToPdf
dotnet remove package TuesPechkin
dotnet remove package Rotativa
dotnet remove package Rotativa.AspNetCore
dotnet remove package NReco.PdfGenerator

# デプロイメントからwkhtmltopdfバイナリを削除
# wkhtmltopdf.exe、wkhtmltox.dllなどを削除

# IronPDFを追加（安全で現代的な代替品）
dotnet add package IronPdf
```

---

## 名前空間マッピング

| wkhtmltopdfラッパー | IronPDF |
|---------------------|---------|
| `WkHtmlToPdfDotNet` | `IronPdf` |
| `DinkToPdf` | `IronPdf` |
| `TuesPechkin` | `IronPdf` |
| `Rotativa` | `IronPdf` |
| `Rotativa.AspNetCore` | `IronPdf` |
| `NReco.PdfGenerator` | `IronPdf` |

---

## APIマッピング

### wkhtmltopdf CLIからIronPDFへ

| wkhtmltopdf CLIオプション | IronPDF相当 | 備考 |
|------------------------|-------------------|-------|
| `wkhtmltopdf input.html output.pdf` | `renderer.RenderHtmlFileAsPdf()` | ファイルからPDFへ |
| `wkhtmltopdf URL output.pdf` | `renderer.RenderUrlAsPdf()` | URLからPDFへ |
| `--page-size A4` | `RenderingOptions.PaperSize = PdfPaperSize.A4` | 用紙サイズ |
| `--page-size Letter` | `RenderingOptions.PaperSize = PdfPaperSize.Letter` | USレター |
| `--orientation Landscape` | `RenderingOptions.PaperOrientation = Landscape` | 方向 |
| `--margin-top 10mm` | `RenderingOptions.MarginTop = 10` | 上余白（mm） |
| `--margin-bottom 10mm` | `RenderingOptions.MarginBottom = 10` | |
| `--margin-left 10mm` | `RenderingOptions.MarginLeft = 10` | |
| `--margin-right 10mm` | `RenderingOptions.MarginRight = 10` | |
| `--header-html header.html` | `RenderingOptions.HtmlHeader` | HTMLヘッダー |
| `--header-center "text"` | `RenderingOptions.TextHeader` | テキストヘッダー |
| `--footer-html footer.html` | `RenderingOptions.HtmlFooter` | HTMLフッター |
| `--footer-center "text"` | `RenderingOptions.TextFooter` | テキストフッター |
| `--footer-center "[page]"` | `{page}`プレースホルダー | ページ番号 |
| `--footer-center "[toPage]"` | `{total-pages}`プレースホルダー | 総ページ数 |
| `--enable-javascript` | デフォルトで有効 | JavaScript |
| `--javascript-delay 500` | `RenderingOptions.WaitFor.RenderDelay = 500` | JS遅延 |
| `--print-media-type` | `RenderingOptions.CssMediaType = Print` | CSSメディア |
| `--dpi 300` | `RenderingOptions.Dpi = 300` | DPI設定 |
| `--grayscale` | `RenderingOptions.GrayScale = true` | グレースケール |
| `--zoom 0.8` | `RenderingOptions.Zoom = 80` | ズーム（%） |
| `--disable-smart-shrinking` | `RenderingOptions.FitToPaperMode` | フィットオプション |
| `--enable-local-file-access` | デフォルトで許可 | ローカルファイル |

### C#ラッパーAPIからIronPDFへ

| wkhtmltopdfラッパー | IronPDF | 備考 |
|---------------------|---------|-------|
| `SynchronizedConverter` | `ChromePdfRenderer` | メインレンダラー |
| `HtmlToPdfDocument` | `RenderingOptions` | 設定 |
| `GlobalSettings.Out` | `pdf.SaveAs()` | 出力ファイル |
| `GlobalSettings.PaperSize` | `RenderingOptions.PaperSize` | 用紙サイズ |
| `GlobalSettings.Orientation` | `RenderingOptions.PaperOrientation` | 方向 |
| `GlobalSettings.Margins` | `RenderingOptions.Margin*` | 個別の余白 |
| `ObjectSettings.Page` | `RenderHtmlFileAsPdf()` | ファイル入力 |
| `ObjectSettings.HtmlContent` | `RenderHtmlAsPdf()` | HTML文字列 |
| `HeaderSettings.Center` | `TextHeader.CenterText` | ヘッダーテキスト |
| `FooterSettings.Center` | `TextFooter.CenterText` | フッターテキスト |
| `converter.Convert(doc)` | `renderer.RenderHtmlAsPdf()` | PDF生成 |

---

## コード例

### 例1: 基本的なHTMLからPDFへ

**以前（wkhtmltopdf CLI）：**
```bash
wkhtmltopdf input.html output.pdf
```

**以前（C#ラッパー - WkHtmlToPdf-DotNet）：**
```csharp
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using System.IO;

var converter = new SynchronizedConverter(new PdfTools());
var doc = new HtmlToPdfDocument()
{
    GlobalSettings = {
        ColorMode = ColorMode.Color,
        Orientation = Orientation.Portrait,
        PaperSize = PaperKind.A4
    },
    Objects = {
        new ObjectSettings()
        {
            Page = "input.html"
        }
    }
};
byte[] pdf = converter.Convert(doc);
File.WriteAllBytes("output.pdf", pdf);
```

**その後（IronPDF）：**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlFileAsPdf("input.html");
pdf.SaveAs("output.pdf");
```

### 例2: HTML文字列からPDFへ

**以前（wkhtmltopdf CLI）：**
```bash
echo "<h1>Hello World</h1>" | wkhtmltopdf - output.pdf
```

**以前（C#ラッパー）：**
```csharp
using WkHtmlToPdfDotNet;
using System.IO;

var converter = new SynchronizedConverter(new PdfTools());
var doc = new HtmlToPdfDocument()
{
    GlobalSettings = {
        ColorMode = ColorMode.Color,
        Orientation = Orientation.Portrait,
        PaperSize = PaperKind.A4
    },
    Objects = {
        new ObjectSettings()
        {
            HtmlContent = "<h1>Hello World</h1>"
        }
    }
};
byte[] pdf = converter.Convert(doc);
File.WriteAllBytes("output.pdf", pdf);
```

**その後（IronPDF）：**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello World</h1>");
pdf.SaveAs("output.pdf");
```

### 例3: URLからPDFへ

**以前（wkhtmltopdf CLI）：**
```bash
wkhtmltopdf https://www.example.com output.pdf
```

**以前（C#ラッパー）：**
```csharp
using WkHtmlToPdfDotNet;
using System.IO;

var converter = new SynchronizedConverter(new PdfTools());
var doc = new HtmlToPdfDocument()
{
    GlobalSettings = {
        ColorMode = ColorMode.Color,
        PaperSize = PaperKind.A4
    },
    Objects = {
        new ObjectSettings()
        {
            Page = "https://www.example.com"
        }
    }
};
byte[] pdf = converter.Convert(doc);
File.WriteAllBytes("output.pdf", pdf);
```

**その後（IronPDF）：**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderUrlAsPdf("https://www.example.com");
pdf.SaveAs("output.pdf");
```

### 例4: カスタムページ設定と余白

**以前（wkhtmltopdf CLI）：**
```bash
wkhtmltopdf \
    --page-size A4 \
    --orientation Landscape \
    --margin-top 20mm \
    --margin-bottom 20mm \
    --margin-left 15mm \
    --margin-right 15mm \
    input.html output.pdf
```

**以前（C#ラッパー）：**
```csharp
using WkHtmlToPdfDotNet;
using System.IO;

var converter = new SynchronizedConverter(new PdfTools());
var doc = new HtmlToPdfDocument()
{
    GlobalSettings = {
        ColorMode = ColorMode.Color,
        Orientation = Orientation.Landscape,
        PaperSize = PaperKind.A4,
        Margins = new MarginSettings()
        {
            Top = 20,
            Bottom = 20,
            Left = 15,
            Right = 15
        }
    },
    Objects = {
        new ObjectSettings()
        {
            Page = "input.html"
        }
    }
};
byte[] pdf = converter.Convert(doc);
File.WriteAllBytes("output.pdf", pdf);
```

**その後（IronPDF）：**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

var renderer = new ChromePdfRenderer();

renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
renderer.RenderingOptions.MarginTop = 20;
renderer.RenderingOptions.MarginBottom = 20;
renderer.RenderingOptions.MarginLeft = 15;
renderer.RenderingOptions.MarginRight = 15;

var pdf = renderer.RenderHtmlFileAsPdf("input.html");
pdf.SaveAs("output.pdf");
```

### 例5: ページ番号付きのヘッダーとフッター

**以前（wkhtmltopdf CLI）：**
```bash
wkhtmltopdf \
    --margin-top 25mm \
    --margin-bottom 20mm \
    --header-center "Company Report" \
    --header-font-size 10 \
    --header-spacing 5 \
    --footer-center "Page [page] of [toPage]" \
    --footer-font-size 8 \
    input.html output.pdf
```

**以前（C#ラッパー）：**
```csharp
using WkHtmlToPdfDotNet;
using System.IO;

var converter = new SynchronizedConverter(new PdfTools());
var doc = new HtmlToPdfDocument()
{
    GlobalSettings = {
        ColorMode = ColorMode.Color,
        PaperSize = PaperKind.A4,
        Marg