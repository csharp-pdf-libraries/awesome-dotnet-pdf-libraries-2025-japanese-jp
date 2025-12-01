---
**🌐 日本語版** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/pdf-to-image-csharp.md)

---

# C#でPDFを画像に変換：PNG、JPEG、サムネイルガイド

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFのクリエーター

[![.NET](https://img.shields.io/badge/.NET-6%2B-512BD4)](https://dotnet.microsoft.com/)
[![最終更新](https://img.shields.io/badge/Updated-November%202025-green)]()

> PDFページを画像に変換することで、サムネイル、プレビュー、画像ベースの処理が可能になります。このガイドでは、解像度制御とライブラリの比較によるラスタライズについて説明します。

---

## 目次

1. [クイックスタート](#クイックスタート)
2. [ライブラリ比較](#ライブラリ比較)
3. [PDFからPNGへ](#pdfからpngへ)
4. [PDFからJPEGへ](#pdfからjpegへ)
5. [解像度と品質](#解像度と品質)
6. [サムネイル](#サムネイル)
7. [バッチ変換](#バッチ変換)
8. [一般的な使用例](#一般的な使用例)

---

## クイックスタート

### IronPDFを使用したPDFからPNGへの変換

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// すべてのページをPNG画像に変換
pdf.RasterizeToImageFiles("output/page_*.png");
```

### シングルページを画像に変換

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// 最初のページのみを変換
var images = pdf.ToBitmap();
images[0].Save("first-page.png");
```

---

## ライブラリ比較

### 画像への変換機能

| ライブラリ | PNGへ | JPEGへ | DPI制御 | サムネイル | 品質制御 |
|---------|--------|---------|-------------|------------|-----------------|
| **IronPDF** | ✅ シンプル | ✅ | ✅ | ✅ | ✅ |
| Aspose.PDF | ✅ | ✅ | ✅ | ✅ | ✅ |
| PDFSharp | ⚠️ 限定的 | ⚠️ | ⚠️ | ⚠️ | ⚠️ |
| Docotic.Pdf | ✅ | ✅ | ✅ | ✅ | ✅ |
| PuppeteerSharp | ⚠️ スクリーンショット | ⚠️ | ⚠️ | ⚠️ | ⚠️ |
| QuestPDF | ❌ | ❌ | ❌ | ❌ | ❌ |
| iText7 | ❌ | ❌ | ❌ | ❌ | ❌ |

**重要な洞察:** iText7はネイティブにPDFを画像に変換できません。QuestPDFは生成のみです。

### コードの複雑さ

**IronPDF — 2行:**
```csharp
var pdf = PdfDocument.FromFile("doc.pdf");
pdf.RasterizeToImageFiles("page_*.png");
```

**Aspose.PDF — 10+行:**
```csharp
var document = new Aspose.Pdf.Document("doc.pdf");
for (int pageCount = 1; pageCount <= document.Pages.Count; pageCount++)
{
    using var imageStream = new FileStream($"page_{pageCount}.png", FileMode.Create);
    var resolution = new Aspose.Pdf.Devices.Resolution(300);
    var pngDevice = new Aspose.Pdf.Devices.PngDevice(resolution);
    pngDevice.Process(document.Pages[pageCount], imageStream);
}
```

---

## PDFからPNGへ

### すべてのページ

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("report.pdf");

// ワイルドカードパターンはpage_1.png、page_2.pngなどを作成します。
pdf.RasterizeToImageFiles("output/page_*.png");

Console.WriteLine($"Converted {pdf.PageCount} pages to PNG");
```

### 特定のページ

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// ページ1、3、5を変換（0から始まるインデックス）
var pageIndexes = new[] { 0, 2, 4 };

foreach (int i in pageIndexes)
{
    var bitmap = pdf.PageToBitmap(i);
    bitmap.Save($"selected_page_{i + 1}.png");
}
```

### メモリへ（バイト配列）

```csharp
using IronPdf;
using System.Drawing.Imaging;

var pdf = PdfDocument.FromFile("document.pdf");

// Web/API使用のためのバイト配列として取得
var imageBytes = new List<byte[]>();

foreach (var bitmap in pdf.ToBitmap())
{
    using var ms = new MemoryStream();
    bitmap.Save(ms, ImageFormat.Png);
    imageBytes.Add(ms.ToArray());
}

// Webレスポンスで使用
// return File(imageBytes[0], "image/png");
```

---

## PDFからJPEGへ

### 基本的なJPEG変換

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// JPEG出力
pdf.RasterizeToImageFiles("output/page_*.jpg");
```

### 品質制御付き

```csharp
using IronPdf;
using System.Drawing;
using System.Drawing.Imaging;

var pdf = PdfDocument.FromFile("document.pdf");

// JPEG品質エンコーダ
var jpegEncoder = ImageCodecInfo.GetImageEncoders()
    .First(e => e.MimeType == "image/jpeg");

var encoderParams = new EncoderParameters(1);
encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 85L); // 85%品質

int page = 1;
foreach (var bitmap in pdf.ToBitmap())
{
    bitmap.Save($"page_{page++}.jpg", jpegEncoder, encoderParams);
}
```

---

## 解像度と品質

### 高解像度（印刷品質）

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// 印刷品質のための300 DPI
pdf.RasterizeToImageFiles("print_quality_*.png", 300);
```

### 低解像度（Webサムネイル）

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// Web使用のための72 DPI - ファイルサイズが小さくなります
pdf.RasterizeToImageFiles("web_*.png", 72);
```

### カスタム解像度

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// 異なる用途のための異なる解像度
var resolutions = new Dictionary<string, int>
{
    { "thumbnail", 72 },
    { "preview", 150 },
    { "print", 300 },
    { "archive", 600 }
};

foreach (var kvp in resolutions)
{
    pdf.RasterizeToImageFiles($"{kvp.Key}/page_*.png", kvp.Value);
}
```

---

## サムネイル

### サムネイルの生成

```csharp
using IronPdf;
using System.Drawing;

var pdf = PdfDocument.FromFile("document.pdf");

// サムネイルのための低DPI
var thumbnails = pdf.ToBitmap(72);

int page = 1;
foreach (var bitmap in thumbnails)
{
    // サムネイル寸法にリサイズ
    var thumb = new Bitmap(bitmap, new Size(150, 200));
    thumb.Save($"thumbnails/thumb_{page++}.png");
}
```

### 最初のページのサムネイルのみ

```csharp
using IronPdf;
using System.Drawing;

var pdf = PdfDocument.FromFile("document.pdf");

// 最初のページを低解像度のサムネイルとして取得
var firstPage = pdf.PageToBitmap(0, 72);

// アスペクト比を維持してリサイズ
int targetWidth = 200;
int targetHeight = (int)(firstPage.Height * (targetWidth / (double)firstPage.Width));

var thumbnail = new Bitmap(firstPage, new Size(targetWidth, targetHeight));
thumbnail.Save("document-thumbnail.png");
```

### サムネイルグリッド

```csharp
using IronPdf;
using System.Drawing;

var pdf = PdfDocument.FromFile("presentation.pdf");

const int thumbWidth = 150;
const int thumbHeight = 200;
const int columns = 4;
const int padding = 10;

var thumbnails = pdf.ToBitmap(72);
int rows = (int)Math.Ceiling(thumbnails.Length / (double)columns);

// グリッド画像を作成
int gridWidth = columns * (thumbWidth + padding) + padding;
int gridHeight = rows * (thumbHeight + padding) + padding;

using var grid = new Bitmap(gridWidth, gridHeight);
using var graphics = Graphics.FromImage(grid);
graphics.Clear(Color.White);

for (int i = 0; i < thumbnails.Length; i++)
{
    int col = i % columns;
    int row = i / columns;

    var thumb = new Bitmap(thumbnails[i], new Size(thumbWidth, thumbHeight));

    int x = padding + col * (thumbWidth + padding);
    int y = padding + row * (thumbHeight + padding);

    graphics.DrawImage(thumb, x, y);
}

grid.Save("thumbnail-grid.png");
```

---

## バッチ変換

### ディレクトリ内のすべてのPDFを変換

```csharp
using IronPdf;

public void ConvertAllPdfs(string inputDir, string outputDir)
{
    var pdfFiles = Directory.GetFiles(inputDir, "*.pdf");

    foreach (var pdfPath in pdfFiles)
    {
        string baseName = Path.GetFileNameWithoutExtension(pdfPath);
        string outputSubDir = Path.Combine(outputDir, baseName);
        Directory.CreateDirectory(outputSubDir);

        var pdf = PdfDocument.FromFile(pdfPath);
        pdf.RasterizeToImageFiles(Path.Combine(outputSubDir, "page_*.png"));

        Console.WriteLine($"Converted: {baseName} ({pdf.PageCount} pages)");
    }
}

// 使用法
ConvertAllPdfs("documents/", "images/");
```

### 並行処理

```csharp
using IronPdf;

public async Task ConvertPdfsParallel(string[] pdfPaths, string outputDir)
{
    await Parallel.ForEachAsync(pdfPaths, async (pdfPath, ct) =>
    {
        var pdf = PdfDocument.FromFile(pdfPath);
        string baseName = Path.GetFileNameWithoutExtension(pdfPath);

        pdf.RasterizeToImageFiles(Path.Combine(outputDir, $"{baseName}_*.png"));

        Console.WriteLine($"Completed: {baseName}");
    });
}
```

---

## 一般的な使用例

### ドキュメントプレビューシステム

```csharp
using IronPdf;
using System.Drawing;

public class DocumentPreviewService
{
    public byte[] GetPreviewImage(string pdfPath, int pageIndex = 0)
    {
        var pdf = PdfDocument.FromFile(pdfPath);

        if (pageIndex >= pdf.PageCount)
            pageIndex = 0;

        var bitmap = pdf.PageToBitmap(pageIndex, 150); // プレビュー用の150 DPI

        using var ms = new MemoryStream();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms.ToArray();
    }

    public int GetPageCount(string pdfPath)
    {
        var pdf = PdfDocument.FromFile(pdfPath);
        return pdf.PageCount;
    }
}
```

### Eコマース製品カタログ

```csharp
using IronPdf;

public void GenerateProductImages(string catalogPdf, string outputDir)
{
    var pdf = PdfDocument.FromFile(catalogPdf);

    // 複数のサイズを生成
    var sizes = new[] { 100, 300, 800 };

    for (int i = 0; i < pdf.PageCount; i++)
    {
        foreach (int size in sizes)
        {
            // 目標幅のためのDPI計算
            int dpi = (int)(size * 72.0 / 612); // レターサイズのPDFを想定

            var bitmap = pdf.PageToBitmap(i, Math.Max(dpi, 72));
            bitmap.Save(Path.Combine(outputDir, $"product_{i + 1}_{size}px.png"));
        }
    }
}
```

### PDFスライドショー

```csharp
using IronPdf;
using System.Drawing;

public class PdfSlideshow
{
    private readonly List<Bitmap> _slides;
    private int _currentSlide;

    public PdfSlideshow(string pdfPath)
    {
        var pdf = PdfDocument.FromFile(pdfPath);
        _slides = pdf.ToBitmap(150).ToList();
        _currentSlide = 0;
    }

    public Bitmap GetCurrentSlide() => _slides[_currentSlide];

    public Bitmap NextSlide()
    {
        _currentSlide = (_currentSlide + 1) % _slides.Count;
        return GetCurrentSlide();
    }

    public Bitmap PreviousSlide()
    {
        _currentSlide = (_currentSlide - 1 + _slides.Count) % _slides.Count;
        return GetCurrentSlide();
    }

    public int TotalSlides => _slides.Count;
    public int CurrentIndex => _currentSlide + 1;
}
```

---

## 推奨事項

### PDFから画像への変換にIronPDFを選ぶ場合:
- ✅ シンプルな2行API
- ✅ 解像度/DPI制御が必要な場合
- ✅ 他のPDF操作と組み合わせる場合
- ✅ クロスプラットフォーム展開

### Docotic.Pdfを選ぶ場合:
- 非常に特定のレンダリング要件がある場合
- 高品質のカラーマネジメントが必要な場合

### PDFを画像に変換できない場合:
- ❌ iText7 — ネイティブのラスタライズがない
- ❌ QuestPDF — 生成のみ
- ❌ PDFSharp — 非常に限定的なサポート

---

## 関連チュートリアル

- **[PDFのマージ](merge-split-pdf-csharp.md)** — 変換前に結合
- **[テキストの抽出](extract-text-from-pdf-csharp.md)** — 変換された画像からOCR
- **[PDFにウォーターマークを追加](watermark-pdf-csharp.md)** — 変換前にウォーターマークを追加
- **[最高のPDFライブラリ](best-pdf-libraries-dotnet-2025.md)** — 完全な比較

---

### その他のチュートリアル
- **[HTMLからPDFへ](html-to-pdf-csharp.md)** — 変換するPDFを生成
- **[ASP.NET Core](asp-net-core-pdf-reports.md)** — Web画像生成
- **[クロスプラットフォーム](cross-platform-pdf-dotnet.md)** — 変換サービスを展開
- **[IronPDFガイド](ironpdf/)** — 完全なラスタライゼーションAPI

---

*「[Awesome .NET PDF Libraries 2025](README.md)」コレクションの一部 — 73のC#/.NET PDFライブラリを比較*