---
**  (Japanese Translation)**

 **English:** [pdfium/migrate-from-pdfium.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/pdfium/migrate-from-pdfium.md)
 **:** [pdfium/migrate-from-pdfium.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/pdfium/migrate-from-pdfium.md)

---
# Pdfium.NETからIronPDFへの移行方法は？

## なぜPdfium.NETからIronPDFに移行するのか？

Pdfium.NETはGoogleのPDFiumライブラリの.NETラッパーであり、PDFのレンダリングには優れていますが、現代のアプリケーションニーズには大きく制限があります。PDFの表示には優れていますが、PDFを作成、編集、または操作することはできません。

### Pdfium.NETの重要な制限

1. **レンダリングのみ**：HTML、画像、またはプログラムからPDFを作成できない
2. **PDF操作不可**：PDFの内容をマージ、分割、または変更できない
3. **ネイティブバイナリ依存**：プラットフォーム固有のPDFiumバイナリが必要
4. **デプロイメントの複雑さ**：プラットフォームごとにネイティブDLLをバンドルして管理する必要がある
5. **限定的なテキスト抽出**：書式なしの基本的なテキスト抽出のみ
6. **HTMLからPDFへの変換不可**：WebコンテンツをPDFに変換できない
7. **ヘッダー/フッター不可**：ページ番号や繰り返しコンテンツを追加できない
8. **ウォーターマーク不可**：オーバーレイでドキュメントにスタンプを押すことができない
9. **フォームサポート不可**：PDFフォームを記入または読み取ることができない
10. **セキュリティ機能不可**：PDFを暗号化またはパスワード保護できない

### IronPDFの利点

| 項目 | Pdfium.NET | IronPDF |
|--------|------------|---------|
| **主な焦点** | レンダリング/表示 | 完全なPDFソリューション |
| **PDF作成** | ✗ | ✓ (HTML、URL、画像) |
| **PDF操作** | ✗ | ✓ (マージ、分割、編集) |
| **HTMLからPDFへ** | ✗ | ✓ (Chromiumエンジン) |
| **ウォーターマーク** | ✗ | ✓ |
| **ヘッダー/フッター** | ✗ | ✓ |
| **フォーム記入** | ✗ | ✓ |
| **セキュリティ** | ✗ | ✓ |
| **ネイティブ依存性** | 必要 | なし (完全に管理された) |
| **クロスプラットフォーム** | 設定が複雑 | 自動 |

---

## NuGetパッケージの変更

```bash
# Pdfiumパッケージを削除
dotnet remove package Pdfium.NET
dotnet remove package Pdfium.Net.SDK
dotnet remove package PdfiumViewer

# IronPDFをインストール
dotnet add package IronPdf
```

---

## 名前空間の変更

```csharp
// Pdfium.NET
using Pdfium;
using Pdfium.Net;
using PdfiumViewer;

// IronPDF
using IronPdf;
using IronPdf.Rendering;
using IronPdf.Editing;
```

---

## 完全なAPIリファレンス

### コアクラスのマッピング

| Pdfium.NET | IronPDF | メモ |
|------------|---------|-------|
| `PdfDocument` | `PdfDocument` | 同じ名前、異なる機能 |
| `PdfPage` | `PdfPage` | 類似のインターフェース |
| `PdfPageCollection` | `PdfPageCollection` | 類似のインターフェース |
| _(利用不可)_ | `ChromePdfRenderer` | PDF作成 |
| _(利用不可)_ | `HtmlHeaderFooter` | ヘッダー/フッター |

### ドキュメントの読み込み

| Pdfium.NET | IronPDF | メモ |
|------------|---------|-------|
| `PdfDocument.Load(path)` | `PdfDocument.FromFile(path)` | ファイルから読み込み |
| `PdfDocument.Load(stream)` | `PdfDocument.FromStream(stream)` | ストリームから読み込み |
| `PdfDocument.Load(bytes)` | `PdfDocument.FromBinaryData(bytes)` | バイトから読み込み |
| `new PdfDocument(path)` | `PdfDocument.FromFile(path)` | コンストラクターパターン |

### ドキュメントのプロパティ

| Pdfium.NET | IronPDF | メモ |
|------------|---------|-------|
| `document.PageCount` | `document.PageCount` | 同じ |
| `document.Pages` | `document.Pages` | 類似のコレクション |
| `document.Pages[index]` | `document.Pages[index]` | ゼロベース |
| `document.GetPageSize(index)` | `document.Pages[index].Width/Height` | 直接のプロパティ |

### ページのレンダリング

| Pdfium.NET | IronPDF | メモ |
|------------|---------|-------|
| `page.Render(width, height)` | `pdf.RasterizeToImageFiles(path, dpi)` | ラスタライズ |
| `page.Render(width, height, flags)` | DPIパラメータ | 品質管理 |
| `document.Render(index, width, height)` | `pdf.RasterizeToImageFiles()` | 一括レンダリング |
| `page.RenderToScale(scale)` | DPI: `72 * scale` | DPIにスケール |

### テキストの抽出

| Pdfium.NET | IronPDF | メモ |
|------------|---------|-------|
| `document.GetPdfText(pageIndex)` | `document.Pages[index].Text` | ページごと |
| _(手動ループ)_ | `document.ExtractAllText()` | 全ページ |
| `page.GetTextBounds()` | `page.Text` | 簡略化 |

### ドキュメントの保存

| Pdfium.NET | IronPDF | メモ |
|------------|---------|-------|
| `document.Save(path)` | `document.SaveAs(path)` | 異なるメソッド名 |
| `document.Save(stream)` | `document.Stream` | ストリームへのアクセス |
| _(利用不可)_ | `document.BinaryData` | バイトを取得 |

### 新機能 (Pdfium.NETにはない)

| IronPDFの機能 | 説明 |
|-----------------|-------------|
| `ChromePdfRenderer.RenderHtmlAsPdf()` | HTMLから作成 |
| `ChromePdfRenderer.RenderUrlAsPdf()` | URLから作成 |
| `ChromePdfRenderer.RenderHtmlFileAsPdf()` | HTMLファイルから作成 |
| `PdfDocument.Merge()` | PDFを結合 |
| `pdf.CopyPages()` | ページを抽出 |
| `pdf.RemovePages()` | ページを削除 |
| `pdf.InsertPdf()` | 位置にPDFを挿入 |
| `pdf.ApplyWatermark()` | ウォーターマークを追加 |
| `pdf.AddHtmlHeaders()` | ヘッダーを追加 |
| `pdf.AddHtmlFooters()` | フッターを追加 |
| `pdf.SecuritySettings` | パスワード保護 |
| `pdf.SignWithDigitalSignature()` | デジタル署名 |
| `pdf.Form` | フォーム記入 |

---

## コード移行例

### 例1: PDFを画像にレンダリング

**移行前 (Pdfium.NET):**
```csharp
using Pdfium;
using System.Drawing;

public class PdfRenderService
{
    public void RenderPdfToImages(string pdfPath, string outputFolder)
    {
        using (var document = PdfDocument.Load(pdfPath))
        {
            for (int i = 0; i < document.PageCount; i++)
            {
                using (var page = document.Pages[i])
                {
                    // 特定のサイズでレンダリング
                    int width = (int)(page.Width * 2); // 2倍のスケール
                    int height = (int)(page.Height * 2);

                    using (var bitmap = page.Render(width, height, PdfRenderFlags.Annotations))
                    {
                        bitmap.Save($"{outputFolder}/page_{i + 1}.png");
                    }
                }
            }
        }
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public class PdfRenderService
{
    public PdfRenderService()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
    }

    public void RenderPdfToImages(string pdfPath, string outputFolder)
    {
        var pdf = PdfDocument.FromFile(pdfPath);

        // 150 DPIで全ページをレンダリング (デフォルトの72 DPIの2倍)
        pdf.RasterizeToImageFiles($"{outputFolder}/page_*.png", DPI: 150);
    }
}
```

### 例2: PDFからテキストを抽出

**移行前 (Pdfium.NET):**
```csharp
using Pdfium;
using System.Text;

public string ExtractText(string pdfPath)
{
    var sb = new StringBuilder();

    using (var document = PdfDocument.Load(pdfPath))
    {
        for (int i = 0; i < document.PageCount; i++)
        {
            string pageText = document.GetPdfText(i);
            sb.AppendLine(pageText);
        }
    }

    return sb.ToString();
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public string ExtractText(string pdfPath)
{
    var pdf = PdfDocument.FromFile(pdfPath);
    return pdf.ExtractAllText();
}

// 必要に応じてページごとに
public string ExtractTextPerPage(string pdfPath)
{
    var pdf = PdfDocument.FromFile(pdfPath);
    var sb = new StringBuilder();

    foreach (var page in pdf.Pages)
    {
        sb.AppendLine(page.Text);
    }

    return sb.ToString();
}
```

### 例3: ページの寸法を取得

**移行前 (Pdfium.NET):**
```csharp
using Pdfium;

public void GetPageInfo(string pdfPath)
{
    using (var document = PdfDocument.Load(pdfPath))
    {
        Console.WriteLine($"Total pages: {document.PageCount}");

        for (int i = 0; i < document.PageCount; i++)
        {
            using (var page = document.Pages[i])
            {
                double width = page.Width;
                double height = page.Height;
                Console.WriteLine($"Page {i + 1}: {width} x {height} points");
            }
        }
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public void GetPageInfo(string pdfPath)
{
    var pdf = PdfDocument.FromFile(pdfPath);

    Console.WriteLine($"Total pages: {pdf.PageCount}");

    for (int i = 0; i < pdf.PageCount; i++)
    {
        var page = pdf.Pages[i];
        Console.WriteLine($"Page {i + 1}: {page.Width} x {page.Height} points");
    }
}
```

### 例4: HTMLからPDFを作成 (新機能)

**移行前 (Pdfium.NET):**
```csharp
// Pdfium.NETはHTMLからPDFを作成できない
// PDFを生成するために別のライブラリが必要
throw new NotSupportedException("Pdfium.NETはHTMLからPDFを作成できない");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public void CreatePdfFromHtml(string html, string outputPath)
{
    var renderer = new ChromePdfRenderer();

    // オプションを設定
    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    renderer.RenderingOptions.MarginTop = 20;
    renderer.RenderingOptions.MarginBottom = 20;

    var pdf = renderer.RenderHtmlAsPdf(html);
    pdf.SaveAs(outputPath);
}
```

### 例5: URLからPDFを作成 (新機能)

**移行前 (Pdfium.NET):**
```csharp
// Pdfium.NETはURLをPDFに変換できない
throw new NotSupportedException("Pdfium.NETはURLをPDFに変換できない");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public void CaptureWebPage(string url, string outputPath)
{
    var renderer = new ChromePdfRenderer();

    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    renderer.RenderingOptions.PrintHtmlBackgrounds = true;
    renderer.RenderingOptions.WaitFor.RenderDelay(1000); // JSの待機

    var pdf = renderer.RenderUrlAsPdf(url);
    pdf.SaveAs(outputPath);
}
```

### 例6: PDFをマージ (新機能)

**移行前 (Pdfium.NET):**
```csharp
// Pdfium.NETはPDFをマージできない
// iTextSharpやPdfSharpのような別のライブラリを使用する必要がある
throw new NotSupportedException("Pdfium.NETはPDFをマージできない");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;
using System.Collections.Generic;

public void MergePdfs(List<string> inputPaths, string outputPath)
{
    var pdfs = inputPaths.Select(path => PdfDocument.FromFile(path)).ToList();
    var merged = PdfDocument.Merge(pdfs);
    merged.SaveAs(outputPath);
}
```

### 例7: ウォーターマークを追加 (新機能)

**移行前 (Pdfium.NET):**
```csharp
// Pdfium.NETはウォーターマークを追加できない
throw new NotSupportedException("Pdfium.NETはウォーターマークを追加できない");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;
using IronPdf.Editing;

public void AddWatermark(string pdfPath, string outputPath)
{
    var pdf = PdfDocument.FromFile(pdfPath);

    pdf.ApplyWatermark(
        "<div style='color:red; font-size:72px; opacity:0.3; transform:rotate(-45deg);'>CONFIDENTIAL</div>",
        45,
        VerticalAlignment.Middle,
        HorizontalAlignment.Center);

    pdf.SaveAs(outputPath);
}
```

### 例8: ヘッダーとフッターを追加 (新機能)

**移行前 (Pdfium.NET):**
```csharp
// Pdfium.NETはヘッダー/フッターを追加できない
throw new NotSupportedException("Pdfium.NETはヘッダー/フッターを追加できない");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public void AddHeadersFooters(string pdfPath, string outputPath)
{
    var pdf = PdfDocument.FromFile(pdfPath);

    pdf.AddHtmlHeaders(new HtmlHeaderFooter
    {
        HtmlFragment = "<div style='text-align:center; font-size:12px;'>Company Report</div>",
        MaxHeight = 25
    });

    pdf.AddHtmlFooters(new HtmlHeaderFooter
    {
        HtmlFragment = "<div style='text-align:center; font-size:10px;'>Page {page} of {total-pages}</div>",
        MaxHeight = 25
    });

    pdf.SaveAs(outputPath);
}
```

### 例9: パスワード保護 (新機能)

**移行前 (Pdfium.NET):**
```csharp
// Pdfium.NETはPDFを暗号化できない
throw new NotSupportedException("Pdfium.NETはPDFを暗号化できない");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public void SecurePdf(string pdfPath, string outputPath)
{
    var pdf = PdfDocument.FromFile(pdfPath);

    pdf.SecuritySettings.OwnerPassword = "admin123";
    pdf.SecuritySettings.UserPassword = "user456";
    pdf.SecuritySettings.AllowUserCopyPasteContent = false;
    pdf.SecuritySettings.AllowUserPrinting = IronPdf.Security.PdfPrintSecurity.NoPrint;

    pdf.SaveAs(outputPath);
}
```

### 例10: PDFフォームを記入 (新機能)

**移行前 (Pdfium.NET):**
```csharp
// Pdfium.NETはPDFフォームを記入できない
throw new NotSupportedException("Pdfium.NETはPDFフォームを記入できない");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public void FillForm(string pdfPath, string