# PDF Duo .NETからIronPDFへの移行方法は？

## なぜ移行するのか？

PDF Duo .NETは、あまり知られていない、文書化が不十分で、メンテナンス状況が不明瞭なライブラリです。IronPDFへの移行により、安定した、よく文書化され、積極的にメンテナンスされているPDF生成ソリューションを提供します。

### PDF Duo .NETの重大な問題

1. **不明瞭な出所**: 開発者不明、会社のバックアップが不明
   - GitHubリポジトリやソースコードが見当たらない
   - NuGetのダウンロード統計が限られている
   - ライセンス条項が不確か

2. **文書が不足**: 信頼できる情報を見つけることがほぼ不可能
   - 公式APIリファレンスがない
   - コミュニティの例が少ない
   - 公式チュートリアルやガイドがない

3. **放棄されたか非活動的**: 放置の兆候
   - 断続的または更新なし
   - サポートフォーラムが非活動的（2019年の投稿）
   - 問題や質問に対する反応がない

4. **機能が限定的**: 基本的な機能のみ
   - シンプルなHTMLからPDFへ
   - 基本的なPDFのマージ
   - 高度な機能（フォーム、セキュリティ、ウォーターマーク）がない

5. **不明なレンダリングエンジン**: 内部の仕組みが不明
   - CSS/JavaScriptのサポートが不明
   - レンダリング品質が予測不可能
   - 現代のウェブ機能が不確か

6. **サポートリスク**: 物事が壊れたときの救済策がない
   - 専門的なサポートがない
   - 助けてくれるコミュニティがない
   - 完全に放棄されるリスク

### IronPDFの利点

| 項目 | PDF Duo .NET | IronPDF |
|--------|-------------|---------|
| メンテナンス | 不明/非活動的 | アクティブ、定期的な更新 |
| 文書 | 不足/欠落 | 包括的 |
| サポート | なし | 専門のサポートチーム |
| コミュニティ | 約0ユーザー | 41M+のNuGetダウンロード |
| レンダリング | 不明なエンジン | 現代のChromium |
| 機能 | 基本的 | 全機能 |
| 安定性 | 不明 | 実績のある生産 |
| ライセンス | 不明瞭 | 透明 |

---

## NuGetパッケージの変更

```bash
# PDF Duo .NETを削除（正しいパッケージ名が見つかる場合）
dotnet remove package PDFDuo.NET
dotnet remove package PDFDuo
dotnet remove package PDF-Duo

# IronPDFをインストール
dotnet add package IronPdf
```

---

## 名前空間の変更

| PDF Duo .NET | IronPDF |
|--------------|---------|
| `using PDFDuo;` | `using IronPdf;` |
| `using PDFDuo.Document;` | `using IronPdf;` |
| `using PDFDuo.Rendering;` | `using IronPdf.Rendering;` |
| `using PDFDuo.Settings;` | `using IronPdf;` |

---

## 完全なAPIマッピング

### HTMLからPDFへの変換

| PDF Duo .NET | IronPDF | 備考 |
|--------------|---------|-------|
| `new HtmlToPdfConverter()` | `new ChromePdfRenderer()` | メインレンダラー |
| `converter.ConvertHtmlString(html, path)` | `renderer.RenderHtmlAsPdf(html).SaveAs(path)` | HTML文字列 |
| `converter.ConvertUrl(url, path)` | `renderer.RenderUrlAsPdf(url).SaveAs(path)` | URL変換 |
| `converter.ConvertFile(htmlPath, pdfPath)` | `renderer.RenderHtmlFileAsPdf(htmlPath).SaveAs(pdfPath)` | HTMLファイル |
| `PDFConverter.FromHtml(html)` | `renderer.RenderHtmlAsPdf(html)` | 静的メソッド |
| `PDFConverter.FromUrl(url)` | `renderer.RenderUrlAsPdf(url)` | 静的メソッド |

### ページ設定

| PDF Duo .NET | IronPDF | 備考 |
|--------------|---------|-------|
| `settings.PageSize = PageSize.A4` | `RenderingOptions.PaperSize = PdfPaperSize.A4` | 用紙サイズ |
| `settings.PageSize = PageSize.Letter` | `RenderingOptions.PaperSize = PdfPaperSize.Letter` | USレター |
| `settings.Orientation = Landscape` | `RenderingOptions.PaperOrientation = Landscape` | 方向 |
| `settings.Margins = new Margins(t, r, b, l)` | 個別のマージンプロパティ | 下記参照 |

### マージン

| PDF Duo .NET | IronPDF | 備考 |
|--------------|---------|-------|
| `new Margins(top, right, bottom, left)` | 個別のプロパティ | マージンオブジェクトなし |
| `margins.Top` | `RenderingOptions.MarginTop` | 上マージン（mm） |
| `margins.Right` | `RenderingOptions.MarginRight` | 右マージン（mm） |
| `margins.Bottom` | `RenderingOptions.MarginBottom` | 下マージン（mm） |
| `margins.Left` | `RenderingOptions.MarginLeft` | 左マージン（mm） |

### ドキュメント操作

| PDF Duo .NET | IronPDF | 備考 |
|--------------|---------|-------|
| `PDFDocument.Load(path)` | `PdfDocument.FromFile(path)` | PDFをロード |
| `document.Save(path)` | `pdf.SaveAs(path)` | PDFを保存 |
| `document.ToBytes()` | `pdf.BinaryData` | バイト配列を取得 |
| `PDFDocument.Merge(docs)` | `PdfDocument.Merge(pdfs)` | PDFをマージ |
| `new PdfMerger()` | `PdfDocument.Merge()` | 静的メソッド |
| `merger.AddFile(path)` | _(別途ロード)_ | ロードしてからマージ |

### PDF Duoにない機能（IronPDFで新規）

| 機能 | IronPDF |
|---------|---------|
| ヘッダー/フッター | `RenderingOptions.HtmlHeader`, `HtmlFooter` |
| ページ番号 | `{page}`, `{total-pages}`プレースホルダー |
| ウォーターマーク | `pdf.ApplyWatermark(html)` |
| パスワード保護 | `pdf.SecuritySettings` |
| フォーム入力 | `pdf.Form.Fields` |
| デジタル署名 | `pdf.SignWithFile()` |
| テキスト抽出 | `pdf.ExtractAllText()` |
| PDFから画像へ | `pdf.RasterizeToImageFiles()` |

---

## コード移行例

### 例1: 基本的なHTMLからPDFへ

**移行前 (PDF Duo .NET):**
```csharp
using PDFDuo;

public class PdfDuoService
{
    public void CreatePdf(string html, string outputPath)
    {
        var converter = new HtmlToPdfConverter();
        converter.ConvertHtmlString(html, outputPath);
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public class PdfService
{
    private readonly ChromePdfRenderer _renderer;

    public PdfService()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
        _renderer = new ChromePdfRenderer();
    }

    public void CreatePdf(string html, string outputPath)
    {
        var pdf = _renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs(outputPath);
    }
}
```

### 例2: 設定付きのURLからPDFへ

**移行前 (PDF Duo .NET):**
```csharp
using PDFDuo;

var settings = new PDFSettings
{
    PageSize = PageSize.A4,
    Margins = new Margins(20, 20, 20, 20),
    Orientation = Orientation.Portrait
};

var converter = new HtmlToPdfConverter(settings);
converter.ConvertUrl("https://example.com", "webpage.pdf");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.MarginTop = 20;
renderer.RenderingOptions.MarginBottom = 20;
renderer.RenderingOptions.MarginLeft = 20;
renderer.RenderingOptions.MarginRight = 20;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;

var pdf = renderer.RenderUrlAsPdf("https://example.com");
pdf.SaveAs("webpage.pdf");
```

### 例3: PDFのマージ

**移行前 (PDF Duo .NET):**
```csharp
using PDFDuo;

var merger = new PdfMerger();
merger.AddFile("document1.pdf");
merger.AddFile("document2.pdf");
merger.AddFile("document3.pdf");
merger.Merge("merged.pdf");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;
using System.Linq;

var paths = new[] { "document1.pdf", "document2.pdf", "document3.pdf" };
var pdfs = paths.Select(PdfDocument.FromFile).ToList();
var merged = PdfDocument.Merge(pdfs);
merged.SaveAs("merged.pdf");
```

### 例4: ヘッダーとフッターの追加（新機能）

**移行前 (PDF Duo .NET):**
```csharp
// PDF Duo .NETではヘッダー/フッターはサポートされていません
// 同等の機能なし
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = "<div style='text-align:center; font-size:10px;'>Company Report</div>",
    MaxHeight = 25
};

renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = "<div style='text-align:center; font-size:10px;'>Page {page} of {total-pages}</div>",
    MaxHeight = 25
};

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("report.pdf");
```

### 例5: ウォーターマークの追加（新機能）

**移行前 (PDF Duo .NET):**
```csharp
// PDF Duo .NETではウォーターマークはサポートされていません
// 同等の機能なし
```

**移行後 (IronPDF):**
```csharp
using IronPdf;
using IronPdf.Editing;

var pdf = PdfDocument.FromFile("document.pdf");

pdf.ApplyWatermark(
    "<h1 style='color:red; opacity:0.5; font-size:72px;'>CONFIDENTIAL</h1>",
    45,
    VerticalAlignment.Middle,
    HorizontalAlignment.Center);

pdf.SaveAs("watermarked.pdf");
```

### 例6: パスワード保護の追加（新機能）

**移行前 (PDF Duo .NET):**
```csharp
// PDF Duo .NETではパスワード保護はサポートされていません
// 同等の機能なし
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

pdf.SecuritySettings.OwnerPassword = "admin123";
pdf.SecuritySettings.UserPassword = "user123";
pdf.SecuritySettings.AllowUserPrinting = PdfPrintSecurity.FullPrintRights;
pdf.SecuritySettings.AllowUserEdits = PdfEditSecurity.NoEdit;
pdf.SecuritySettings.AllowUserCopyPasteContent = false;

pdf.SaveAs("protected.pdf");
```

### 例7: テキスト抽出（新機能）

**移行前 (PDF Duo .NET):**
```csharp
// PDF Duo .NETではテキスト抽出はサポートされていません
// 同等の機能なし
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");
string allText = pdf.ExtractAllText();
Console.WriteLine(allText);

// ページごとの抽出
for (int i = 0; i < pdf.PageCount; i++)
{
    string pageText = pdf.ExtractTextFromPage(i);
    Console.WriteLine($"Page {i + 1}: {pageText}");
}
```

### 例8: PDFから画像へ（新機能）

**移行前 (PDF Duo .NET):**
```csharp
// PDF Duo .NETではPDFから画像への変換はサポートされていません
// 同等の機能なし
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// すべてのページを画像としてレンダリング
pdf.RasterizeToImageFiles("page_*.png", DPI: 150);

// またはビットマップとして取得
var bitmaps = pdf.ToBitmap(150);
```

### 例9: フォーム入力（新機能）

**移行前 (PDF Duo .NET):**
```csharp
// PDF Duo .NETではフォーム入力はサポートされていません
// 同等の機能なし
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("form.pdf");

// フォームフィールドを入力
pdf.Form.FindFormField("Name").Value = "John Doe";
pdf.Form.FindFormField("Email").Value = "john@example.com";
pdf.Form.FindFormField("Date").Value = DateTime.Now.ToString("yyyy-MM-dd");

// 必要に応じてフラット化（編集不可にする）
pdf.Form.Flatten();

pdf.SaveAs("filled_form.pdf");
```

### 例10: 完全な移行例

**移行前 (PDF Duo .NET):**
```csharp
using PDFDuo;

public class PdfDuoService
{
    public void GenerateReport(string html, string outputPath)
    {
        var settings = new PDFSettings
        {
            PageSize = PageSize.A4,
            Margins = new Margins(20, 20, 20, 20)
        };

        var converter = new HtmlToPdfConverter(settings);
        converter.ConvertHtmlString(html, outputPath);

        // それだけ - ヘッダー、フッター、ウォーターマーク、セキュリティなし
        // PDF Duo .NETの機能は非常に限定的
    }

    public void MergePdfs(string[] files, string outputPath)
    {
        var merger = new PdfMerger();
        foreach (var file in files)
        {
            merger.AddFile(file);
        }
        merger.Merge(outputPath);
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;
using IronPdf.Editing;
using System.Linq;

public class PdfService
{
    private readonly ChromePdfRenderer _renderer;

    public PdfService()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
        _renderer = new ChromePdfRenderer();

        // レンダラーを一度設定し、すべてのレンダリングで再利用
        _renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        _renderer.RenderingOptions.MarginTop = 20;
        _renderer.RenderingOptions.MarginBottom = 20;
        _renderer.RenderingOptions.MarginLeft = 20;
        _renderer.RenderingOptions.MarginRight = 20;

        // プロフェッショナルなヘッダーとフッターを追加
        _renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
        {
            HtmlFragment = "<div style='text-align:center;'>Company Report</div>",