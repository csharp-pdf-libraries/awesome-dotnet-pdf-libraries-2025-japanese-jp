---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [pdfiumviewer/migrate-from-pdfiumviewer.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/pdfiumviewer/migrate-from-pdfiumviewer.md)
🇯🇵 **日本語:** [pdfiumviewer/migrate-from-pdfiumviewer.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/pdfiumviewer/migrate-from-pdfiumviewer.md)

---

# PDFiumViewerからIronPDFへの移行方法は？

## なぜPDFiumViewerからIronPDFに移行するのか？

PDFiumViewerは、GoogleのPDFiumレンダリングエンジン用の.NETラッパーで、Windows Forms PDFビュー専用に設計されています。PDFの表示には優れていますが、PDFを作成、編集、操作することはできず、そのメンテナンス状況が不確かであるため、本番アプリケーションにリスクをもたらします。

### PDFiumViewerの重要な制限

1. **表示のみ**: HTML、画像、またはプログラムからPDFを作成できない
2. **Windows Formsのみ**: WinFormsアプリケーションに限定
3. **PDF操作不可**: PDFの内容をマージ、分割、または変更できない
4. **ネイティブバイナリ依存**: プラットフォーム固有のPDFiumバイナリが必要
5. **メンテナンスが不確か**: 更新が限定的で長期的なサポートが不明確
6. **テキスト抽出不可**: PDFからテキストを抽出できない（画像としてのみレンダリング）
7. **HTMLからPDFへの変換不可**: WebコンテンツをPDFに変換できない
8. **ヘッダー/フッター不可**: ページ番号や繰り返しコンテンツを追加できない
9. **ウォーターマーク不可**: 文書にオーバーレイでスタンプを押すことができない
10. **セキュリティ機能不可**: PDFを暗号化またはパスワード保護できない

### IronPDFの利点

| 項目 | PDFiumViewer | IronPDF |
|--------|--------------|---------|
| **主な焦点** | WinForms PDFビューア | 完全なPDFソリューション |
| **PDF作成** | ✗ | ✓ (HTML、URL、画像) |
| **PDF操作** | ✗ | ✓ (マージ、分割、編集) |
| **HTMLからPDFへ** | ✗ | ✓ (Chromiumエンジン) |
| **テキスト抽出** | ✗ | ✓ |
| **ウォーターマーク** | ✗ | ✓ |
| **ヘッダー/フッター** | ✗ | ✓ |
| **セキュリティ** | ✗ | ✓ |
| **プラットフォームサポート** | Windows Formsのみ | コンソール、Web、デスクトップ |
| **フレームワークサポート** | .NET Framework | .NET Framework、Core、5+ |
| **メンテナンス** | 不確か | アクティブ |

---

## NuGetパッケージの変更

```bash
# PDFiumViewerパッケージを削除
dotnet remove package PdfiumViewer
dotnet remove package PdfiumViewer.Native.x86.v8-xfa
dotnet remove package PdfiumViewer.Native.x64.v8-xfa

# IronPDFをインストール
dotnet add package IronPdf
```

---

## 名前空間の変更

```csharp
// PDFiumViewer
using PdfiumViewer;

// IronPDF
using IronPdf;
using IronPdf.Rendering;
using IronPdf.Editing;
```

---

## 完全なAPIリファレンス

### コアクラスのマッピング

| PDFiumViewer | IronPDF | 備考 |
|--------------|---------|-------|
| `PdfDocument` | `PdfDocument` | 名前は同じだが、機能が異なる |
| `PdfViewer` | _(該当なし)_ | IronPDFはバックエンドに焦点 |
| `PdfRenderer` | `ChromePdfRenderer` | PDF作成 |
| _(利用不可)_ | `HtmlHeaderFooter` | ヘッダー/フッター |

### ドキュメントの読み込み

| PDFiumViewer | IronPDF | 備考 |
|--------------|---------|-------|
| `PdfDocument.Load(path)` | `PdfDocument.FromFile(path)` | ファイルから読み込み |
| `PdfDocument.Load(stream)` | `PdfDocument.FromStream(stream)` | ストリームから読み込み |
| `PdfDocument.Load(bytes)` | `PdfDocument.FromBinaryData(bytes)` | バイトから読み込み |

### ドキュメントのプロパティ

| PDFiumViewer | IronPDF | 備考 |
|--------------|---------|-------|
| `document.PageCount` | `document.PageCount` | 同じ |
| `document.PageSizes` | `document.Pages[i].Width/Height` | ページごとのアクセス |
| `document.GetPageSize(index)` | `document.Pages[index].Width/Height` | 直接プロパティ |

### ページのレンダリング

| PDFiumViewer | IronPDF | 備考 |
|--------------|---------|-------|
| `document.Render(pageIndex, dpiX, dpiY, forPrinting)` | `pdf.RasterizeToImageFiles(path, dpi)` | ラスタライズ |
| `document.Render(pageIndex, width, height, dpiX, dpiY, flags)` | DPIパラメータ | 品質管理 |
| `PdfRenderFlags` enum | DPIパラメータ | 簡略化 |

### ドキュメントの保存

| PDFiumViewer | IronPDF | 備考 |
|--------------|---------|-------|
| `document.Save(path)` | `pdf.SaveAs(path)` | メソッド名が異なる |
| `document.Save(stream)` | `pdf.Stream` | ストリームへのアクセス |
| _(利用不可)_ | `pdf.BinaryData` | バイトを取得 |

### ビューアコントロール (WinForms)

| PDFiumViewer | IronPDFの代替 | 備考 |
|--------------|---------------------|-------|
| `PdfViewer` コントロール | 保存 + 外部ビューア | IronPDFにはUIコントロールがない |
| `pdfViewer.Document = doc` | `pdf.SaveAs(path)` その後ビュー | バックエンドに焦点 |
| `pdfViewer.Zoom` | 該当なし | ビューア固有のコントロールを使用 |

### 新機能 (PDFiumViewerにはない)

| IronPDFの機能 | 説明 |
|-----------------|-------------|
| `ChromePdfRenderer.RenderHtmlAsPdf()` | HTMLから作成 |
| `ChromePdfRenderer.RenderUrlAsPdf()` | URLから作成 |
| `PdfDocument.Merge()` | PDFを結合 |
| `pdf.CopyPages()` | ページを抽出 |
| `pdf.RemovePages()` | ページを削除 |
| `pdf.ApplyWatermark()` | ウォーターマークを追加 |
| `pdf.AddHtmlHeaders()` | ヘッダーを追加 |
| `pdf.AddHtmlFooters()` | フッターを追加 |
| `pdf.SecuritySettings` | パスワード保護 |
| `pdf.ExtractAllText()` | テキストを抽出 |
| `pdf.Form` | フォーム入力 |

---

## コード移行の例

### 例1: PDFを読み込み、画像にレンダリング

**以前 (PDFiumViewer):**
```csharp
using PdfiumViewer;
using System.Drawing;
using System.Drawing.Imaging;

public class PdfRenderService
{
    public void RenderPdfToImages(string pdfPath, string outputFolder)
    {
        using (var document = PdfDocument.Load(pdfPath))
        {
            for (int i = 0; i < document.PageCount; i++)
            {
                // 150 DPIでレンダリング
                using (var image = document.Render(i, 150, 150, true))
                {
                    image.Save($"{outputFolder}/page_{i + 1}.png", ImageFormat.Png);
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

        // 150 DPIで全ページをレンダリング
        pdf.RasterizeToImageFiles($"{outputFolder}/page_*.png", DPI: 150);
    }
}
```

### 例2: Windows Forms PDFビューア

**以前 (PDFiumViewer):**
```csharp
using PdfiumViewer;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private PdfViewer pdfViewer;

    public MainForm()
    {
        InitializeComponent();

        pdfViewer = new PdfViewer();
        pdfViewer.Dock = DockStyle.Fill;
        this.Controls.Add(pdfViewer);
    }

    private void OpenPdf(string path)
    {
        var document = PdfDocument.Load(path);
        pdfViewer.Document = document;
    }

    private void btnOpen_Click(object sender, EventArgs e)
    {
        using (var dialog = new OpenFileDialog())
        {
            dialog.Filter = "PDF files (*.pdf)|*.pdf";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                OpenPdf(dialog.FileName);
            }
        }
    }
}
```

**移行後 (IronPDF - バックエンド処理):**
```csharp
using IronPdf;
using System.Windows.Forms;
using System.Diagnostics;

public partial class MainForm : Form
{
    // IronPDFはバックエンドに焦点 - WebBrowserコントロールまたは外部ビューアを使用

    private WebBrowser webBrowser;
    private string tempPdfPath;

    public MainForm()
    {
        InitializeComponent();
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        webBrowser = new WebBrowser();
        webBrowser.Dock = DockStyle.Fill;
        this.Controls.Add(webBrowser);
    }

    private void OpenPdf(string path)
    {
        // オプション1: デフォルトのPDFビューアで開く
        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });

        // オプション2: WebBrowserコントロールで表示（Adobeプラグインがインストールされている場合）
        webBrowser.Navigate(path);
    }

    // IronPDFは、PDFiumViewerが提供できなかった機能を有効にします:
    private void CreatePdfFromHtml(string html)
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf(html);
        tempPdfPath = System.IO.Path.GetTempFileName() + ".pdf";
        pdf.SaveAs(tempPdfPath);
        OpenPdf(tempPdfPath);
    }
}
```

### 例3: ページ情報の取得

**以前 (PDFiumViewer):**
```csharp
using PdfiumViewer;

public void GetPageInfo(string pdfPath)
{
    using (var document = PdfDocument.Load(pdfPath))
    {
        Console.WriteLine($"Total pages: {document.PageCount}");

        for (int i = 0; i < document.PageCount; i++)
        {
            var size = document.PageSizes[i];
            Console.WriteLine($"Page {i + 1}: {size.Width} x {size.Height} points");
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

### 例4: テキストの抽出 (新機能)

**以前 (PDFiumViewer):**
```csharp
// PDFiumViewerはテキストを抽出できません
// OCRまたは別のライブラリが必要です
throw new NotSupportedException("PDFiumViewerはPDFからテキストを抽出できません");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public string ExtractText(string pdfPath)
{
    var pdf = PdfDocument.FromFile(pdfPath);
    return pdf.ExtractAllText();
}

public string ExtractTextFromPage(string pdfPath, int pageIndex)
{
    var pdf = PdfDocument.FromFile(pdfPath);
    return pdf.Pages[pageIndex].Text;
}
```

### 例5: HTMLからPDFを作成 (新機能)

**以前 (PDFiumViewer):**
```csharp
// PDFiumViewerはPDFを作成できません
throw new NotSupportedException("PDFiumViewerはHTMLからPDFを作成できません");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public void CreatePdfFromHtml(string html, string outputPath)
{
    var renderer = new ChromePdfRenderer();

    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    renderer.RenderingOptions.MarginTop = 20;
    renderer.RenderingOptions.MarginBottom = 20;

    // ページ番号を追加
    renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
    {
        HtmlFragment = "<div style='text-align:center;'>Page {page} of {total-pages}</div>",
        MaxHeight = 20
    };

    var pdf = renderer.RenderHtmlAsPdf(html);
    pdf.SaveAs(outputPath);
}
```

### 例6: PDFのマージ (新機能)

**以前 (PDFiumViewer):**
```csharp
// PDFiumViewerはPDFをマージできません
throw new NotSupportedException("PDFiumViewerはPDFをマージできません");
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

### 例7: ウォーターマークの追加 (新機能)

**以前 (PDFiumViewer):**
```csharp
// PDFiumViewerはウォーターマークを追加できません
throw new NotSupportedException("PDFiumViewerはウォーターマークを追加できません");
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

### 例8: PDFの印刷

**以前 (PDFiumViewer):**
```csharp
using PdfiumViewer;
using System.Drawing.Printing;

public void PrintPdf(string pdfPath, string printerName)
{
    using (var document = PdfDocument.Load(pdfPath))
    {
        using (var printDocument = document.CreatePrintDocument())
        {
            printDocument.PrinterSettings.PrinterName = printerName;
            printDocument.Print();
        }
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public void PrintPdf(string pdfPath, string printerName)
{
    var pdf = PdfDocument.FromFile(pdfPath);
    pdf.Print(printerName);
}
```

### 例9: パスワード保護 (新機能)

**以前 (PDFiumViewer):**
```csharp
// PDFiumViewerはPDFを暗号化できません
throw new NotSupportedException("PDFiumViewerはPDFを暗号化できません");
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

### 例10: URLをPDFに変換 (新機能)

**以前 (PDFiumViewer):**
```csharp
// PDFiumViewerはURLを