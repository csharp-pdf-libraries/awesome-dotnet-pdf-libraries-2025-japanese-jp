---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [nutrient/migrate-from-nutrient.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/nutrient/migrate-from-nutrient.md)
🇯🇵 **日本語:** [nutrient/migrate-from-nutrient.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/nutrient/migrate-from-nutrient.md)

---

# Nutrient（旧PSPDFKit）からIronPDFへの移行方法は？

## なぜ移行するのか？

Nutrient（旧PSPDFKit）はPDFライブラリからAI機能とエンタープライズの複雑さを備えた包括的な「ドキュメントインテリジェンスプラットフォーム」へと進化しました。プラットフォームのオーバーヘッドなしに直接的なPDF操作が必要なチームに対して、IronPDFは集中的でコスト効果の高い代替手段を提供します。

### Nutrientの問題点

1. **プラットフォームの過剰設計**: かつてのPDF SDKは現在、「ドキュメントインテリジェンスプラットフォーム」になっています
   - 必要としないかもしれないAI機能
   - PDFを超えるドキュメントワークフロー機能
   - 単純なPDFタスクのための複雑なアーキテクチャ

2. **エンタープライズ価格**: 大企業向けに位置付けられています
   - 営業担当者への連絡が必要な不透明な価格設定
   - 中小規模のチームにとって高価
   - クラウド/サーバー展開のための複雑なライセンス

3. **リブランドによる混乱**: PSPDFKit → Nutrientへの移行
   - 文書化は両方の名前を参照しています
   - パッケージ名はまだPSPDFKitを使用している可能性があります
   - 移行中に移行パスが不明確

4. **非同期ファーストの複雑さ**: すべてがasync/awaitを必要とします
   - 初期化のための`PdfProcessor.CreateAsync()`
   - 単純なタスクのための非同期操作
   - 同期ワークフローのためのオーバーヘッド

5. **重い依存関係**: フルプラットフォームはより多くのリソースを必要とします
   - 大きなパッケージフットプリント
   - より多くの初期化時間
   - 追加の設定

### IronPDFの利点

| アスペクト | Nutrient (PSPDFKit) | IronPDF |
|--------|-------------------|---------|
| フォーカス | ドキュメントインテリジェンスプラットフォーム | PDFライブラリ |
| 価格設定 | エンタープライズ（営業に連絡） | 透明で公開されている |
| アーキテクチャ | 複雑なプラットフォーム | 単純なライブラリ |
| APIスタイル | 非同期ファースト | 同期と非同期のオプション |
| 依存関係 | 重い | 軽量 |
| 設定 | 複雑 | 単純 |
| 学習曲線 | 急（プラットフォーム） | 緩やか（ライブラリ） |
| 対象ユーザー | エンタープライズ | すべてのチームサイズ |

---

## NuGetパッケージの変更

```bash
# Nutrient/PSPDFKitパッケージを削除
dotnet remove package PSPDFKit.NET
dotnet remove package PSPDFKit.PDF
dotnet remove package Nutrient
dotnet remove package Nutrient.PDF

# IronPDFをインストール
dotnet add package IronPdf
```

---

## 名前空間の変更

| Nutrient (PSPDFKit) | IronPDF |
|---------------------|---------|
| `using PSPDFKit.Pdf;` | `using IronPdf;` |
| `using PSPDFKit.Pdf.Document;` | `using IronPdf;` |
| `using PSPDFKit.Pdf.Rendering;` | `using IronPdf.Rendering;` |
| `using PSPDFKit.Pdf.Forms;` | `using IronPdf.Forms;` |
| `using PSPDFKit.Pdf.Annotation;` | `using IronPdf;` |
| `using Nutrient.Pdf;` | `using IronPdf;` |

---

## 完全なAPIマッピング

### 初期化

| Nutrient (PSPDFKit) | IronPDF | 備考 |
|---------------------|---------|-------|
| `await PdfProcessor.CreateAsync()` | `new ChromePdfRenderer()` | 非同期は不要 |
| `processor.Dispose()` | _(自動または手動)_ | よりシンプルなライフサイクル |
| `new PdfConfiguration { ... }` | `renderer.RenderingOptions` | プロパティベース |

### ドキュメントの読み込み

| Nutrient (PSPDFKit) | IronPDF | 備考 |
|---------------------|---------|-------|
| `await processor.OpenAsync(path)` | `PdfDocument.FromFile(path)` | デフォルトで同期 |
| `Document.Load(path)` | `PdfDocument.FromFile(path)` | 同じパターン |
| `Document.LoadFromStream(stream)` | `PdfDocument.FromStream(stream)` | ストリームサポート |
| `Document.LoadFromBytes(bytes)` | `new PdfDocument(bytes)` | バイト配列 |

### PDF生成

| Nutrient (PSPDFKit) | IronPDF | 備考 |
|---------------------|---------|-------|
| `await processor.GeneratePdfFromHtmlStringAsync(html)` | `renderer.RenderHtmlAsPdf(html)` | 同期メソッド |
| `await processor.GeneratePdfFromUrlAsync(url)` | `renderer.RenderUrlAsPdf(url)` | 直接URL |
| `await processor.GeneratePdfFromFileAsync(path)` | `renderer.RenderHtmlFileAsPdf(path)` | HTMLファイル |
| `PdfProcessor.CreatePdfFromUrl(url, config)` | `renderer.RenderUrlAsPdf(url)` | よりシンプル |

### ページ設定

| Nutrient (PSPDFKit) | IronPDF | 備考 |
|---------------------|---------|-------|
| `config.PageSize = PageSize.A4` | `RenderingOptions.PaperSize = PdfPaperSize.A4` | Enum |
| `config.Orientation = Orientation.Landscape` | `RenderingOptions.PaperOrientation = Landscape` | Enum |
| `config.Margins = new Margins(t, r, b, l)` | 個別のマージンプロパティ | MarginTopなど |

### ドキュメント操作

| Nutrient (PSPDFKit) | IronPDF | 備考 |
|---------------------|---------|-------|
| `await processor.MergeAsync(docs)` | `PdfDocument.Merge(pdfs)` | 同期 |
| `document.PageCount` | `pdf.PageCount` | 同じパターン |
| `document.GetPages()` | `pdf.Pages` | コレクション |
| `document.AddPage(page)` | `pdf.AppendPdf(otherPdf)` | 追加 |
| `await document.RemovePageAsync(index)` | `pdf.RemovePages(index)` | 同期 |

### 注釈と透かし

| Nutrient (PSPDFKit) | IronPDF | 備考 |
|---------------------|---------|-------|
| `await document.AddAnnotationAsync(index, annotation)` | `pdf.ApplyWatermark(html)` | HTMLベース |
| `new TextAnnotation("text")` | HTMLでの透かし | より柔軟 |
| `annotation.Opacity = 0.5` | CSS `opacity: 0.5` | CSSスタイリング |
| `annotation.FontSize = 48` | CSS `font-size: 48px` | CSSスタイリング |

### フォーム処理

| Nutrient (PSPDFKit) | IronPDF | 備考 |
|---------------------|---------|-------|
| `document.GetFormFields()` | `pdf.Form.Fields` | フォームコレクション |
| `field.SetValue(value)` | `field.Value = value` | プロパティ |
| `document.FlattenAnnotations()` | `pdf.Form.Flatten()` | フォームをフラット化 |

### ヘッダーとフッター

| Nutrient (PSPDFKit) | IronPDF | 備考 |
|---------------------|---------|-------|
| _(複雑な注釈アプローチ)_ | `RenderingOptions.HtmlHeader` | 単純なHTML |
| _(複雑な注釈アプローチ)_ | `RenderingOptions.HtmlFooter` | 単純なHTML |
| _(カスタム実装)_ | `{page}`プレースホルダー | ページ番号 |
| _(カスタム実装)_ | `{total-pages}`プレースホルダー | 合計ページ数 |

### 出力

| Nutrient (PSPDFKit) | IronPDF | 備考 |
|---------------------|---------|-------|
| `await document.SaveAsync(path)` | `pdf.SaveAs(path)` | 同期 |
| `document.ToBytes()` | `pdf.BinaryData` | バイト配列 |
| `document.ToStream()` | `pdf.Stream` | ストリーム |

---

## コード移行例

### 例1: 基本的なHTMLからPDFへ

**移行前 (Nutrient/PSPDFKit):**
```csharp
using PSPDFKit.Pdf;
using System.Threading.Tasks;

public class NutrientService
{
    public async Task<byte[]> GeneratePdfAsync(string html)
    {
        using var processor = await PdfProcessor.CreateAsync();

        var document = await processor.GeneratePdfFromHtmlStringAsync(html);
        await document.SaveAsync("output.pdf");

        return await document.ToBytesAsync();
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

    public byte[] GeneratePdf(string html)
    {
        var pdf = _renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs("output.pdf");
        return pdf.BinaryData;
    }
}
```

### 例2: 設定付きのURLからPDFへ

**移行前 (Nutrient/PSPDFKit):**
```csharp
using PSPDFKit.Pdf;
using System.Threading.Tasks;

public async Task<byte[]> ConvertUrlAsync(string url)
{
    var configuration = new PdfConfiguration
    {
        PageSize = PageSize.A4,
        Orientation = Orientation.Portrait,
        Margins = new Margins(20, 20, 20, 20),
        JavaScriptEnabled = true,
        WaitForLoadingComplete = true
    };

    using var processor = await PdfProcessor.CreateAsync();
    var document = await processor.CreatePdfFromUrlAsync(url, configuration);

    return await document.ToBytesAsync();
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public byte[] ConvertUrl(string url)
{
    var renderer = new ChromePdfRenderer();

    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;
    renderer.RenderingOptions.MarginTop = 20;
    renderer.RenderingOptions.MarginBottom = 20;
    renderer.RenderingOptions.MarginLeft = 20;
    renderer.RenderingOptions.MarginRight = 20;
    renderer.RenderingOptions.EnableJavaScript = true;
    renderer.RenderingOptions.WaitFor.RenderDelay(2000);

    var pdf = renderer.RenderUrlAsPdf(url);
    return pdf.BinaryData;
}
```

### 例3: PDFのマージ

**移行前 (Nutrient/PSPDFKit):**
```csharp
using PSPDFKit.Pdf;
using System.Collections.Generic;
using System.Threading.Tasks;

public async Task MergePdfsAsync(string[] inputPaths, string outputPath)
{
    using var processor = await PdfProcessor.CreateAsync();

    var documents = new List<PdfDocument>();
    foreach (var path in inputPaths)
    {
        var doc = await processor.OpenAsync(path);
        documents.Add(doc);
    }

    var mergedDocument = await processor.MergeAsync(documents);
    await mergedDocument.SaveAsync(outputPath);

    foreach (var doc in documents)
    {
        doc.Dispose();
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;
using System.Linq;

public void MergePdfs(string[] inputPaths, string outputPath)
{
    var pdfs = inputPaths.Select(PdfDocument.FromFile).ToList();
    var merged = PdfDocument.Merge(pdfs);
    merged.SaveAs(outputPath);
}
```

### 例4: 透かしの追加

**移行前 (Nutrient/PSPDFKit):**
```csharp
using PSPDFKit.Pdf;
using PSPDFKit.Pdf.Annotation;
using System.Threading.Tasks;

public async Task AddWatermarkAsync(string inputPath, string outputPath)
{
    using var processor = await PdfProcessor.CreateAsync();
    var document = await processor.OpenAsync(inputPath);

    for (int i = 0; i < document.PageCount; i++)
    {
        var watermark = new TextAnnotation("CONFIDENTIAL")
        {
            Opacity = 0.5f,
            FontSize = 48,
            Color = new Color(128, 128, 128),
            Rotation = 45,
            Position = new Position(
                document.GetPage(i).Width / 2,
                document.GetPage(i).Height / 2
            )
        };

        await document.AddAnnotationAsync(i, watermark);
    }

    await document.SaveAsync(outputPath);
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;
using IronPdf.Editing;

public void AddWatermark(string inputPath, string outputPath)
{
    var pdf = PdfDocument.FromFile(inputPath);

    pdf.ApplyWatermark(
        "<h1 style='color:gray; opacity:0.5; font-size:48px;'>CONFIDENTIAL</h1>",
        45, // 回転
        VerticalAlignment.Middle,
        HorizontalAlignment.Center);

    pdf.SaveAs(outputPath);
}
```

### 例5: ヘッダーとフッター

**移行前 (Nutrient/PSPDFKit):**
```csharp
using PSPDFKit.Pdf;
using System.Threading.Tasks;

// Nutrientはヘッダー/フッターに複雑な注釈ベースのアプローチが必要です
// しばしばカスタム実装または後処理が必要です
public async Task CreateWithHeaderFooterAsync(string html, string outputPath)
{
    using var processor = await PdfProcessor.CreateAsync();
    var document = await processor.GeneratePdfFromHtmlStringAsync(html);

    // 複雑: 各ページに手動でテキスト注釈を追加する必要があります
    for (int i = 0; i < document.PageCount; i++)
    {
        var header = new TextAnnotation("Document Header")
        {
            Position = new Position(document.GetPage(i).Width / 2, 20),
            FontSize = 10
        };
        await document.AddAnnotationAsync(i, header);

        var footer = new TextAnnotation($"Page {i + 1}")
        {
            Position = new Position(document.GetPage(i).Width / 2, document.GetPage(i).Height - 20),
            FontSize = 10
        };
        await document.AddAnnotationAsync(i, footer);
    }

    await document.SaveAsync(outputPath);
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public void CreateWithHeaderFooter(string html, string outputPath)
{
    var renderer = new ChromePdfRenderer();

    renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
    {
        HtmlFragment = "<div style='text-align:center; font-size:10px;'>Document Header</div>",
        MaxHeight = 25
    };

    renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
    {
        HtmlFragment = "<div style='text-align:center; font-size:10px;'>Page {page} of {total-pages}</div>",
        MaxHeight = 25
    };

    var pdf = renderer.RenderHtmlAsPdf(html);
    pdf.SaveAs(outputPath);
}
```

### 例6: フォームフィールドの処理

**移行前 (Nutrient/PSPDFKit):**
```csharp
using PSPDFKit.Pdf;
using PSPDFKit.Pdf.Forms;
using System.Threading.Tasks;

public async Task FillFormAsync(string inputPath, string outputPath)
{
    using var processor = await PdfProcessor.CreateAsync();
    var document = await processor.Open