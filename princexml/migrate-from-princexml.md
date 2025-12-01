---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [princexml/migrate-from-princexml.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/princexml/migrate-from-princexml.md)
🇯🇵 **日本語:** [princexml/migrate-from-princexml.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/princexml/migrate-from-princexml.md)

---

# 移行ガイド: PrinceXML → IronPDF

## PrinceXMLから移行する理由は？

### 外部プロセスの問題

PrinceXMLは、**別のコマンドライン実行可能ファイル**として動作し、.NETアプリケーションにとって大きなアーキテクチャ上の課題を生み出します：

1. **プロセス管理のオーバーヘッド**：外部プロセスを生成、監視、終了させる必要がある
2. **ネイティブ.NET統合なし**：stdin/stdoutまたは一時ファイル経由で通信する
3. **デプロイの複雑さ**：すべてのサーバーにPrinceのインストールが必要
4. **サーバーごとのライセンス**：各デプロイメントには別のライセンスが必要
5. **エラーハンドリングの難しさ**：エラー検出のためにテキスト出力を解析する
6. **非同期/待機なし**：ブロッキングコールまたは複雑な非同期ラッパーが必要
7. **パス依存性**：PATHまたは絶対パス上のPrince実行可能ファイルを見つける必要がある

### CSSページメディアの制限

PrinceXMLのCSSページメディアサポートは強力ですが、ベンダーロックインを生み出します：

```css
/* 他では機能しないPrince固有のCSS */
@page {
    size: A4;
    margin: 2cm;
    @top-center {
        content: "ドキュメントタイトル";
    }
    @bottom-right {
        content: counter(page);
    }
}

/* Prince固有の拡張 */
prince-pdf-page-label: "チャプター " counter(chapter);
prince-pdf-destination: attr(id);
```

### 移行比較の概要

| 項目 | PrinceXML | IronPDF |
|--------|-----------|---------|
| アーキテクチャ | 外部プロセス | ネイティブ.NETライブラリ |
| 統合 | コマンドライン | 直接API |
| デプロイメント | すべてのサーバーにインストール | 単一のNuGetパッケージ |
| エラーハンドリング | テキスト出力を解析 | .NET例外 |
| 非同期サポート | 手動ラッパー | ネイティブ非同期/待機 |
| PDF操作 | 生成のみ | 完全な操作 |
| ライセンス | サーバーごと | 開発者ごと |
| 更新 | 手動で再インストール | NuGet更新 |
| デバッグ | 困難 | 完全なデバッガーサポート |

---

## クイックスタート: PrinceXMLからIronPDFへ5分で移行

### ステップ1: IronPDFのインストール

```bash
# IronPDFのインストール
dotnet add package IronPdf

# 使用している場合はPrinceラッパーを削除
dotnet remove package PrinceXMLWrapper
```

### ステップ2: プロセスコードの置換

**PrinceXML:**
```csharp
using System.Diagnostics;

var process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "prince",
        Arguments = "input.html -o output.pdf",
        UseShellExecute = false,
        RedirectStandardError = true
    }
};
process.Start();
process.WaitForExit();

if (process.ExitCode != 0)
{
    throw new Exception(process.StandardError.ReadToEnd());
}
```

**IronPDF:**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "あなたのライセンスキー";
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlFileAsPdf("input.html");
pdf.SaveAs("output.pdf");
```

### ステップ3: CSSページメディアの移行

**PrinceXML CSS:**
```css
@page {
    size: A4;
    margin: 2cm;
}
```

**IronPDF C# (同等):**
```csharp
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.MarginTop = 56;    // ~2cm
renderer.RenderingOptions.MarginBottom = 56;
renderer.RenderingOptions.MarginLeft = 56;
renderer.RenderingOptions.MarginRight = 56;
```

---

## 完全なAPIリファレンス

### コマンドラインからメソッドへのマッピング

| Princeコマンド | IronPDF相当 |
|---------------|-------------------|
| `prince input.html -o output.pdf` | `renderer.RenderHtmlFileAsPdf("input.html").SaveAs("output.pdf")` |
| `prince --style=custom.css input.html` | HTMLにCSSを含めるか`RenderingOptions`を使用 |
| `prince --javascript` | `renderer.RenderingOptions.EnableJavaScript = true` |
| `prince --no-javascript` | `renderer.RenderingOptions.EnableJavaScript = false` |
| `prince --page-size=Letter` | `renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter` |
| `prince --page-margin=1in` | `renderer.RenderingOptions.MarginTop = 72` (72ポイント = 1インチ) |
| `prince --pdf-lang=en` | `renderer.RenderingOptions.PdfTitle = "..."; // メタデータ` |
| `prince --encrypt` | `pdf.SecuritySettings.OwnerPassword = "..."` |
| `prince --user-password=pw` | `pdf.SecuritySettings.UserPassword = "pw"` |
| `prince --disallow-print` | `pdf.SecuritySettings.AllowUserPrinting = PdfPrintSecurity.NoPrint` |
| `prince --disallow-copy` | `pdf.SecuritySettings.AllowUserCopyPasteContent = false` |
| `prince --baseurl=http://...` | `renderer.RenderingOptions.BaseUrl = new Uri("http://...")` |
| `prince --media=print` | `renderer.RenderingOptions.CssMediaType = PdfCssMediaType.Print` |
| `prince --media=screen` | `renderer.RenderingOptions.CssMediaType = PdfCssMediaType.Screen` |

### CSS @pageからRenderingOptionsへのマッピング

| CSS @pageプロパティ | IronPDF相当 |
|-------------------|-------------------|
| `size: A4` | `PaperSize = PdfPaperSize.A4` |
| `size: Letter` | `PaperSize = PdfPaperSize.Letter` |
| `size: A4 landscape` | `PaperSize = PdfPaperSize.A4` + `PaperOrientation = Landscape` |
| `margin: 2cm` | `MarginTop/Bottom/Left/Right = 56` |
| `margin-top: 1in` | `MarginTop = 72` |
| `@top-center { content: "..." }` | 中央揃えのdivを含む`HtmlHeader` |
| `@bottom-right { content: counter(page) }` | `{page}`プレースホルダーを含む`HtmlFooter` |

### ページサイズの変換

| サイズ | ポイント | ミリメートル |
|------|--------|-------------|
| Letter | 612 x 792 | 216 x 279 |
| A4 | 595 x 842 | 210 x 297 |
| Legal | 612 x 1008 | 216 x 356 |
| A3 | 842 x 1191 | 297 x 420 |
| 1インチ | 72 | 25.4 |
| 1cm | 28.35 | 10 |

---

## コード例

### 例1: 基本的なHTMLファイルからPDFへ

**PrinceXML:**
```csharp
using System.Diagnostics;
using System.IO;

public class PrinceConverter
{
    private readonly string _princePath;

    public PrinceConverter(string princePath = "prince")
    {
        _princePath = princePath;
    }

    public void ConvertHtmlToPdf(string htmlPath, string outputPath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _princePath,
                Arguments = $"\"{htmlPath}\" -o \"{outputPath}\"",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Prince変換失敗: {error}");
        }
    }
}

// 使用例
var converter = new PrinceConverter(@"C:\Program Files\Prince\engine\bin\prince.exe");
converter.ConvertHtmlToPdf("report.html", "report.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

public class PdfConverter
{
    public PdfConverter()
    {
        IronPdf.License.LicenseKey = "あなたのライセンスキー";
    }

    public void ConvertHtmlToPdf(string htmlPath, string outputPath)
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlFileAsPdf(htmlPath);
        pdf.SaveAs(outputPath);
    }
}

// 使用例
var converter = new PdfConverter();
converter.ConvertHtmlToPdf("report.html", "report.pdf");
```

### 例2: HTML文字列からPDFへ

**PrinceXML:**
```csharp
using System.Diagnostics;
using System.IO;

public byte[] ConvertHtmlStringToPdf(string htmlContent)
{
    // Princeはファイル入力を必要とする - 一時ファイルを作成する必要がある
    string tempHtmlPath = Path.GetTempFileName() + ".html";
    string tempPdfPath = Path.GetTempFileName() + ".pdf";

    try
    {
        // HTMLを一時ファイルに書き込む
        File.WriteAllText(tempHtmlPath, htmlContent);

        // Princeを実行
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "prince",
                Arguments = $"\"{tempHtmlPath}\" -o \"{tempPdfPath}\"",
                UseShellExecute = false,
                RedirectStandardError = true
            }
        };

        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception(process.StandardError.ReadToEnd());
        }

        // PDFバイトを読み込む
        return File.ReadAllBytes(tempPdfPath);
    }
    finally
    {
        // 一時ファイルをクリーンアップ
        if (File.Exists(tempHtmlPath)) File.Delete(tempHtmlPath);
        if (File.Exists(tempPdfPath)) File.Delete(tempPdfPath);
    }
}
```

**IronPDF:**
```csharp
using IronPdf;

public byte[] ConvertHtmlStringToPdf(string htmlContent)
{
    var renderer = new ChromePdfRenderer();
    var pdf = renderer.RenderHtmlAsPdf(htmlContent);
    return pdf.BinaryData;
}

// または直接保存
public void ConvertHtmlStringToFile(string htmlContent, string outputPath)
{
    var renderer = new ChromePdfRenderer();
    var pdf = renderer.RenderHtmlAsPdf(htmlContent);
    pdf.SaveAs(outputPath);
}
```

### 例3: JavaScriptを使用したURLからPDFへ

**PrinceXML:**
```csharp
using System.Diagnostics;

public void ConvertUrlToPdf(string url, string outputPath)
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "prince",
            Arguments = $"--javascript \"{url}\" -o \"{outputPath}\"",
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        }
    };

    process.Start();

    // Princeは限定的なJavaScriptサポートを持つ
    // 複雑なSPAではタイムアウトする可能性がある
    process.WaitForExit(30000); // 30秒タイムアウト

    if (!process.HasExited)
    {
        process.Kill();
        throw new TimeoutException("Prince変換がタイムアウトしました");
    }

    if (process.ExitCode != 0)
    {
        throw new Exception(process.StandardError.ReadToEnd());
    }
}
```

**IronPDF:**
```csharp
using IronPdf;

public void ConvertUrlToPdf(string url, string outputPath)
{
    var renderer = new ChromePdfRenderer();

    // 完全なJavaScriptサポート (ES2024)
    renderer.RenderingOptions.EnableJavaScript = true;

    // JavaScriptの完了を待つ
    renderer.RenderingOptions.WaitFor.JavaScript(5000);

    // 特定の要素を待つ
    renderer.RenderingOptions.WaitFor.HtmlElementById("content-loaded", 10000);

    // ネットワークのアイドルを待つ
    renderer.RenderingOptions.WaitFor.NetworkIdle0(5000);

    var pdf = renderer.RenderUrlAsPdf(url);
    pdf.SaveAs(outputPath);
}

// 非同期バージョン
public async Task ConvertUrlToPdfAsync(string url, string outputPath)
{
    var renderer = new ChromePdfRenderer();
    renderer.RenderingOptions.EnableJavaScript = true;
    renderer.RenderingOptions.WaitFor.JavaScript(5000);

    var pdf = await renderer.RenderUrlAsPdfAsync(url);
    pdf.SaveAs(outputPath);
}
```

### 例4: CSSページメディアのヘッダー/フッター移行

**PrinceXML CSS:**
```css
@page {
    size: A4;
    margin: 2cm 2cm 3cm 2cm;

    @top-left {
        content: "会社名";
        font-size: 10pt;
        color: #666;
    }

    @top-right {
        content: string(chapter-title);
        font-size: 10pt;
        font-style: italic;
    }

    @bottom-center {
        content: "ページ " counter(page) " の " counter(pages);
        font-size: 9pt;
    }

    @bottom-right {
        content: "生成日: " prince-script(today);
        font-size: 8pt;
        color: #999;
    }
}

h1 { string-set: chapter-title content(); }
```

**IronPDF C#:**
```csharp
using IronPdf;

public PdfDocument ConvertWithHeadersFooters(string html)
{
    var renderer = new ChromePdfRenderer();

    // ページ設定
    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    renderer.RenderingOptions.MarginTop = 80;     // ヘッダー用の余白
    renderer.RenderingOptions.MarginBottom = 100; // フッター用の余白
    renderer.RenderingOptions.MarginLeft = 56;
    renderer.RenderingOptions.MarginRight = 56;

    // ヘッダー（完全なHTML/CSSサポート付き）
    renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
    {
        HtmlFragment = @"
            <div style='width: 100%; font-size: 10pt; display: flex; justify-content: space-between;'>
                <span style='color: #666;'>会社名</span>
                <span style='font-style: italic;'>{html-title}</span>
            </div>",
        MaxHeight = 30
    };

    // ページ番号と日付を含むフッター
    renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
    {
        HtmlFragment = @"
            <div style='width: 100%; font-size: 9pt; display: flex; justify-content: space-between;'>
                <span></span>
                <span>ページ {page} の {total-pages}</span>
                <span style='font-size: 8pt; color: #999;'>生成日: {date}</span>
            </div>",
        MaxHeight = 40
    };

    return renderer.RenderHtmlAsPdf(html);
}
```

### 例5: ページサイズと向き

**PrinceXML:**
```csharp
var process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "prince",
        Arguments = "--page-size=Letter --page-margin=\"0.5in 1in\" " +
                   "--pdf-page-layout=two-column-left input.html -o output.pdf",
        UseShellExecute = false
    }
};
process.Start();
process.WaitForExit();
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

// ページサイズ
renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;

// 余白（ポイント単位：72ポイント = 1インチ）
renderer.RenderingOptions.MarginTop = 36;     // 0.5インチ
renderer.RenderingOptions.MarginBottom = 36;
renderer.RenderingOptions.MarginLeft = 72;    // 1インチ
renderer.RenderingOptions.MarginRight = 72;

// カスタム