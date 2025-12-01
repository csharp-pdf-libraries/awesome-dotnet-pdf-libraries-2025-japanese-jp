---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [pdf-duo/README.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/pdf-duo/README.md)
🇯🇵 **日本語:** [pdf-duo/README.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/pdf-duo/README.md)

---

# PDF Duo .NET + C# + PDF

PDF Duo .NETは、HTMLやその他のフォーマットをPDFに変換することを目的とした、.NETエコシステム内であまり知られていないライブラリです。多くの開発者がC#でのPDF生成にPDF Duo .NETの潜在的な有用性に興味を持つかもしれませんが、このライブラリの曖昧さは大きな課題を提示します。PDF Duo .NETは、限られたドキュメント、コミュニティとの関わりの少なさ、サポートとメンテナンスの不確実性により、本番環境のアプリケーションにはあまり理想的ではありません。

信頼性のある代替手段として多くの開発者が考慮する可能性のある選択肢はIronPDFです。PDF生成の風景において強固な存在感を持ち、詳細なドキュメントとアクティブなサポートネットワークを提供するIronPDFは、PDF機能を必要とする開発者にとって堅実な選択肢を提供します。

## PDF Duo .NETを理解する

PDF Duo .NETの主な魅力は、その広告されたシンプルさにあります — 外部DLL依存関係のオーバーヘッドなしにスリークな機能性を提供するという名目上の約束です。しかし、実際には、このライブラリの主張はその曖昧な状態によって影を落とされています。PDF Duo .NETを利用しようとする試みは、信頼できるドキュメントやコミュニティサポートプラットフォームの不足により、問題解決やより高度な機能の実装において大きな課題に直面します。

このライブラリの強みは、その希薄なドキュメントを効果的に解釈できれば、統合の容易さにあるかもしれません。しかし、更新の欠如と放棄される非常に現実的なリスクは、重要なプロジェクトにとってその実用性を損ないます。

## IronPDF - 強固な代替手段

IronPDFは、Iron Softwareによって開発された、よく文書化され、積極的にメンテナンスされているライブラリとして、鮮明な対照を示しています。多様な機能範囲で高く評価され、包括的なチュートリアルと技術ガイドの基盤に支えられたIronPDFは、HTMLからPDFへの変換に強力なソリューションを提供します。

### IronPDFの主要機能

- **HTMLからPDFへの変換能力**: 複雑なHTML/CSSをサポートするシームレスな変換体験。
- **プロフェッショナルサポート**: 専用のサポートチームによってバックアップされ、問題が迅速に解決されます。
- **定期的な更新**: 最新の技術と環境との互換性を保証します。

資源豊富なドキュメントとプロフェッショナルなサポートネットワークは、PDF Duo .NETの不確実性と比較して、IronPDFを好ましい選択肢とします。

---

## C#でPDF Duo .NETを使ってHTMLをPDFに変換する方法は？

以下が**PDF Duo .NET**での処理方法です：

```csharp
// NuGet: Install-Package PDFDuo.NET
using PDFDuo;
using System;

class Program
{
    static void Main()
    {
        var converter = new HtmlToPdfConverter();
        var htmlContent = "<h1>Hello World</h1><p>This is a PDF document.</p>";
        converter.ConvertHtmlString(htmlContent, "output.pdf");
        Console.WriteLine("PDF created successfully!");
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        var htmlContent = "<h1>Hello World</h1><p>This is a PDF document.</p>";
        var pdf = renderer.RenderHtmlAsPdf(htmlContent);
        pdf.SaveAs("output.pdf");
        Console.WriteLine("PDF created successfully!");
    }
}
```

IronPDFのアプローチは、現代の.NETアプリケーションとのより良い統合とクリーナーな構文を提供し、PDF生成ワークフローのメンテナンスとスケーリングを容易にします。

---

## .NETでURLをPDFに変換する方法は？

以下が**PDF Duo .NET**での処理方法です：

```csharp
// NuGet: Install-Package PDFDuo.NET
using PDFDuo;
using System;

class Program
{
    static you Main()
    {
        var converter = new HtmlToPdfConverter();
        converter.ConvertUrl("https://www.example.com", "webpage.pdf");
        Console.WriteLine("Webpage converted to PDF!");
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderUrlAsPdf("https://www.example.com");
        pdf.SaveAs("webpage.pdf");
        Console.WriteLine("Webpage converted to PDF!");
    }
}
```

IronPDFのアプローチは、現代の.NETアプリケーションとのより良い統合とクリーナーな構文を提供し、PDF生成ワークフローのメンテナンスとスケーリングを容易にします。

---

## PDFのマージ方法は？

以下が**PDF Duo .NET**での処理方法です：

```csharp
// NuGet: Install-Package PDFDuo.NET
using PDFDuo;
using System;

class Program
{
    static void Main()
    {
        var merger = new PdfMerger();
        merger.AddFile("document1.pdf");
        merger.AddFile("document2.pdf");
        merger.Merge("merged.pdf");
        Console.WriteLine("PDFs merged successfully!");
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var pdf1 = PdfDocument.FromFile("document1.pdf");
        var pdf2 = PdfDocument.FromFile("document2.pdf");
        var merged = PdfDocument.Merge(pdf1, pdf2);
        merged.SaveAs("merged.pdf");
        Console.WriteLine("PDFs merged successfully!");
    }
}
```

IronPDFのアプローチは、現代の.NETアプリケーションとのより良い統合とクリーナーな構文を提供し、PDF生成ワークフローのメンテナンスとスケーリングを容易にします。

---

## PDF Duo .NETからIronPDFへの移行方法は？

### PDF Duoリスク問題

PDF Duo .NETは、本番アプリケーションにとって重大なリスクを提示します：

1. **不明瞭な出所**: 開発者/会社不明、検証可能なサポートチャネルなし
2. **放棄された状態**: 最近の更新なし、希薄なドキュメント
3. **不明なレンダリングエンジン**: 基盤技術についての透明性なし
4. **欠落機能**: ヘッダー/フッター、透かし、セキュリティ機能なし
5. **ゼロコミュニティ**: Stack Overflowの存在やコミュニティヘルプほぼなし

### 簡単な移行概要

| 項目 | PDF Duo .NET | IronPDF |
|--------|--------------|---------|
| メンテナンス | 放棄/不明 | アクティブな開発 |
| ドキュメント | ほぼ存在しない | 包括的 |
| レンダリングエンジン | 不明 | Chromiumベース |
| サポート | なし | プロフェッショナルサポート |
| ヘッダー/フッター | サポートされていない | 完全なHTMLサポート |
| 透かし | サポートされていない | HTMLベースの透かし |
| セキュリティ | サポートされていない | 暗号化 & 権限 |
| コミュニティ | なし | アクティブなコミュニティ |

### 主要APIマッピング

| PDF Duo .NET | IronPDF | 備考 |
|--------------|---------|-------|
| `new HtmlToPdfConverter()` | `new ChromePdfRenderer()` | 現代的なレンダラー |
| `converter.ConvertHtmlString(html, path)` | `renderer.RenderHtmlAsPdf(html).SaveAs(path)` | チェーン可能なAPI |
| `converter.ConvertUrl(url, path)` | `renderer.RenderUrlAsPdf(url).SaveAs(path)` | URLレンダリング |
| `converter.ConvertHtmlFile(file, path)` | `renderer.RenderHtmlFileAsPdf(file).SaveAs(path)` | ファイル入力 |
| `new PdfMerger()` | `PdfDocument.Merge()` | 静的メソッド |
| `merger.AddFile(path)` | `PdfDocument.FromFile(path)` | 先にロード |
| `merger.Merge(output)` | `merged.SaveAs(output)` | マージ後 |
| `converter.PageWidth = ...` | `renderer.RenderingOptions.PaperSize` | PdfPaperSizeを使用 |
| `converter.PageHeight = ...` | `renderer.RenderingOptions.SetCustomPaperSize()` | カスタムサイズ |
| `converter.Margins = new Margins(...)` | 個別のマージンプロパティ | MarginTopなど |
| _(利用不可)_ | `HtmlHeaderFooter` | 新機能 |
| _(利用不可)_ | `pdf.ApplyWatermark()` | 新機能 |
| _(利用不可)_ | `pdf.SecuritySettings` | 新機能 |
| _(利用不可)_ | `pdf.ExtractAllText()` | 新機能 |

### 移行コード例

**移行前 (PDF Duo .NET):**
```csharp
using PDFDuo;

public class PdfDuoService
{
    public void CreatePdf(string html, string outputPath)
    {
        var converter = new HtmlToPdfConverter();

        // 限定的な設定オプション
        converter.PageWidth = 8.5f;
        converter.PageHeight = 11f;
        converter.Margins = new Margins(0.5f, 0.5f, 0.5f, 0.5f);

        converter.ConvertHtmlString(html, outputPath);

        // ヘッダー、フッター、透かし、セキュリティなし
        // ページ番号を追加する方法なし
        // テキスト抽出機能なし
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;
using IronPdf.Editing;

public class PdfService
{
    private readonly ChromePdfRenderer _renderer;

    public PdfService()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
        _renderer = new ChromePdfRenderer();

        // 組み込みオプションでの用紙サイズ
        _renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;

        // 個別のマージンプロパティ (mm単位)
        _renderer.RenderingOptions.MarginTop = 15;
        _renderer.RenderingOptions.MarginBottom = 15;
        _renderer.RenderingOptions.MarginLeft = 15;
        _renderer.RenderingOptions.MarginRight = 15;

        // 新機能: ページ番号付きのヘッダーとフッター
        _renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
        {
            HtmlFragment = "<div style='text-align:center; font-size:10px;'>ページ {page} / {total-pages} </div>",
            MaxHeight = 20
        };
    }

    public void CreatePdf(string html, string outputPath)
    {
        var pdf = _renderer.RenderHtmlAsPdf(html);

        // 新機能: 透かしサポート
        pdf.ApplyWatermark("<div style='color:gray; opacity:0.3; font-size:72px;'>機密</div>",
            45,
            VerticalAlignment.Middle,
            HorizontalAlignment.Center);

        // 新機能: セキュリティ設定
        pdf.SecuritySettings.OwnerPassword = "admin123";
        pdf.SecuritySettings.AllowUserPrinting = IronPdf.Security.PdfPrintSecurity.FullPrintRights;

        pdf.SaveAs(outputPath);
    }

    // 新機能: テキスト抽出 (PDF Duoでは不可能)
    public string ExtractText(string pdfPath)
    {
        var pdf = PdfDocument.FromFile(pdfPath);
        return pdf.ExtractAllText();
    }
}
```

### 重要な移行ノート

1. **マージンオブジェクト → 個別プロパティ**:
   ```csharp
   // PDF Duo: converter.Margins = new Margins(top, right, bottom, left)
   // IronPDF: ミリメートル単位の個別プロパティ
   renderer.RenderingOptions.MarginTop = 15;
   renderer.RenderingOptions.MarginBottom = 15;
   ```

2. **ページサイズ → PaperSize Enum**:
   ```csharp
   // PDF Duo: converter.PageWidth = 8.5f; converter.PageHeight = 11f;
   // IronPDF: 組み込みの用紙サイズを使用
   renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;
   ```

3. **マージャーパターン → 静的マージ**:
   ```csharp
   // PDF Duo: merger.AddFile(path); merger.Merge(output);
   // IronPDF: ファイルをロードしてから静的マージ
   var merged = PdfDocument.Merge(pdf1, pdf2);
   ```

4. **新機能 - ページ番号**: `{page}` と `{total-pages}` プレースホルダーを使用
   ```csharp
   HtmlFragment = "ページ {page} / {total-pages}"
   ```

5. **新機能 - 透かし**: 完全なCSSサポートを備えたHTMLベース
   ```csharp
   pdf.ApplyWatermark("<div style='...'>下書き</div>");
   ```

### NuGetパッケージ移行

```bash
# PDF Duo .NETを削除
dotnet remove package PDFDuo.NET

# IronPDFをインストール
dotnet add package IronPdf
```

### PDF Duo参照の検索

```bash
# コードベースでのPDF Duoの使用箇所を検索
grep -r "PDFDuo\|HtmlToPdfConverter\|PdfMerger" --include="*.cs" .