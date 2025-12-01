---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [mupdf/migrate-from-mupdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/mupdf/migrate-from-mupdf.md)
🇯🇵 **日本語:** [mupdf/migrate-from-mupdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/mupdf/migrate-from-mupdf.md)

---

# 移行ガイド: MuPDF (.NET バインディング) → IronPDF

## なぜ移行するのか？

MuPDF は優れた PDF レンダラーですが、AGPL ライセンスとレンダリングのみに焦点を当てたことが、商用アプリケーションを構築する .NET 開発者にとって大きな制限となっています。このガイドでは、IronPDF の包括的な PDF ソリューションへの完全な移行パスを提供します。

### MuPDF に関する重要な問題

1. **AGPL ライセンスの罠**: ウイルス性ライセンスは以下のいずれかを要求します:
   - AGPL の下でアプリケーション全体をオープンソース化
   - 高価な商用ライセンスの購入（営業に連絡、価格設定不透明）

2. **レンダリングのみに焦点**: MuPDF はビューア/レンダラーであり、以下のために設計されていません:
   - HTML からの PDF 作成
   - ドキュメント生成ワークフロー
   - フォームの記入や変更
   - ウォーターマークやヘッダー/フッターの追加

3. **ネイティブ依存関係**: プラットフォーム固有のバイナリが必要です:
   - Windows、Linux、macOS での手動管理
   - ネイティブライブラリを使用した Docker の複雑さ
   - デプロイメントパッケージングの課題

4. **HTML サポートなし**: HTML/CSS を PDF に変換できません - 外部ツールを使用する必要があります

5. **限定的な操作**: 組み込みサポートがない:
   - PDF のマージ/分割
   - ページの回転や並べ替え
   - ウォーターマークや注釈
   - デジタル署名

6. **C インターオプの複雑さ**: ネイティブバインディングは以下を導入します:
   - メモリ管理の懸念
   - プラットフォーム固有のバグ
   - マーシャリングのオーバーヘッド

### IronPDF の利点

| 項目 | MuPDF | IronPDF |
|------|-------|---------|
| ライセンス | AGPL (ウイルス性) または高価な商用 | 透明な価格設定の商用 |
| 主な焦点 | レンダリング/表示 | 完全な PDF ソリューション |
| HTML から PDF | サポートされていない | 完全な Chromium エンジン |
| PDF 作成 | サポートされていない | HTML、URL、画像 |
| PDF 操作 | 限定的 | 完全 (マージ、分割、編集) |
| 依存関係 | ネイティブバイナリ | 完全に管理された |
| プラットフォームサポート | プラットフォームごとに手動 | 自動 |
| 非同期サポート | 限定的 | 完全な非同期/待機 |
| .NET 統合 | C インターオプ | ネイティブ .NET |

---

## NuGet パッケージの変更

```bash
# MuPDF パッケージを削除
dotnet remove package MuPDF.NET
dotnet remove package MuPDFCore
dotnet remove package MuPDFCore.MuPDFWrapper

# IronPDF をインストール
dotnet add package IronPdf
```

**また、デプロイメントからネイティブの MuPDF バイナリを削除してください**:
- `mupdf.dll`、`libmupdf.so`、`libmupdf.dylib` を削除
- プラットフォーム固有のフォルダを削除
- MuPDF のインストールを削除するために Docker ファイルを更新

---

## 名前空間の変更

| MuPDF | IronPDF |
|-------|---------|
| `using MuPDFCore;` | `using IronPdf;` |
| `using MuPDF.NET;` | `using IronPdf;` |
| | `using IronPdf.Rendering;` (列挙型用) |

---

## 完全な API マッピング

### ドキュメントの読み込み

| MuPDF | IronPDF | 備考 |
|-------|---------|-------|
| `new MuPDFDocument(path)` | `PdfDocument.FromFile(path)` | ファイルから読み込み |
| `new MuPDFDocument(stream)` | `PdfDocument.FromStream(stream)` | ストリームから読み込み |
| `new MuPDFDocument(bytes)` | `new PdfDocument(bytes)` | バイトから読み込み |
| `document.Dispose()` | `pdf.Dispose()` | クリーンアップ |

### ページアクセス

| MuPDF | IronPDF | 備考 |
|-------|---------|-------|
| `document.Pages.Count` | `pdf.PageCount` | ページ数 |
| `document.Pages[index]` | `pdf.Pages[index]` | ページへのアクセス |
| `document.LoadPage(index)` | `pdf.Pages[index]` | 特定のページを読み込み |
| `page.Width` | `page.Width` | ページ幅 |
| `page.Height` | `page.Height` | ページ高さ |

### 画像へのレンダリング

| MuPDF | IronPDF | 備考 |
|-------|---------|-------|
| `page.RenderPixMap(dpi, dpi, alpha)` | `pdf.RasterizeToImageFiles(path, dpi)` | 画像へのページのレンダリング |
| `page.ToPixmap(scale)` | `pdf.ToBitmap()` | ビットマップへ |
| `pixmap.SaveAsPng(path)` | `pdf.RasterizeToImageFiles("*.png")` | PNG として保存 |
| `pixmap.SaveAsJpeg(path)` | `pdf.RasterizeToImageFiles("*.jpg")` | JPEG として保存 |

### テキストの抽出

| MuPDF | IronPDF | 備考 |
|-------|---------|-------|
| `page.GetText()` | `page.Text` | ページのテキスト |
| `document.Pages.Select(p => p.GetText())` | `pdf.ExtractAllText()` | 全テキスト |
| _(構造化テキスト)_ | `pdf.ExtractTextFromPage(index)` | ページごとのテキスト |

### PDF 作成 (MuPDF では不可)

| MuPDF | IronPDF | 備考 |
|-------|---------|-------|
| _(サポートされていない)_ | `ChromePdfRenderer.RenderHtmlAsPdf(html)` | HTML から PDF へ |
| _(サポートされていない)_ | `ChromePdfRenderer.RenderUrlAsPdf(url)` | URL から PDF へ |
| _(サポートされていない)_ | `ChromePdfRenderer.RenderHtmlFileAsPdf(path)` | HTML ファイルから PDF へ |
| _(サポートされていない)_ | `ImageToPdfConverter.ImageToPdf(imagePaths)` | 画像から PDF へ |

### PDF 操作 (MuPDF では限定的)

| MuPDF | IronPDF | 備考 |
|-------|---------|-------|
| _(限定的)_ | `PdfDocument.Merge(pdf1, pdf2)` | PDF のマージ |
| _(限定的)_ | `pdf.CopyPages(start, end)` | ページの抽出 |
| _(サポートされていない)_ | `pdf.InsertPdf(otherPdf, index)` | ページの挿入 |
| _(サポートされていない)_ | `pdf.RemovePages(indices)` | ページの削除 |
| _(サポートされていない)_ | `pdf.RotatePages(angle)` | ページの回転 |

### ドキュメントプロパティ

| MuPDF | IronPDF | 備考 |
|-------|---------|-------|
| `document.Metadata` | `pdf.MetaData` | ドキュメントのメタデータ |
| _(読み取り専用)_ | `pdf.MetaData.Title = "..."` | タイトルの設定 |
| _(読み取り専用)_ | `pdf.MetaData.Author = "..."` | 著者の設定 |

### セキュリティ (MuPDF では限定的)

| MuPDF | IronPDF | 備考 |
|-------|---------|-------|
| _(読み取り専用)_ | `pdf.Password = "pass"` | パスワードの設定 |
| _(読み取り専用)_ | `pdf.SecuritySettings` | 権限の設定 |
| _(サポートされていない)_ | `pdf.SignWithFile(certPath, password)` | デジタル署名 |

### 出力

| MuPDF | IronPDF | 備考 |
|-------|---------|-------|
| `document.SaveAs(path)` | `pdf.SaveAs(path)` | ファイルへ保存 |
| _(手動)_ | `pdf.BinaryData` | バイト配列の取得 |
| _(手動)_ | `pdf.Stream` | ストリームの取得 |

---

## コード移行例

### 例 1: PDF ページの読み込みとレンダリング

**移行前 (MuPDF):**
```csharp
using MuPDFCore;
using System;

public class MuPdfRenderer
{
    public void RenderPdfToImages(string pdfPath, string outputFolder)
    {
        using (var context = new MuPDFContext())
        using (var document = new MuPDFDocument(context, pdfPath))
        {
            for (int i = 0; i < document.Pages.Count; i++)
            {
                // 150 DPI でレンダリング
                using (var page = document.Pages[i])
                {
                    var pixmap = page.RenderPixMap(150, 150, false);
                    var outputPath = Path.Combine(outputFolder, $"page_{i + 1}.png");
                    pixmap.SaveAsPng(outputPath);
                    pixmap.Dispose();
                }
            }
        }
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;
using System.IO;

public class PdfRenderer
{
    public void RenderPdfToImages(string pdfPath, string outputFolder)
    {
        var pdf = PdfDocument.FromFile(pdfPath);

        // 150 DPI で全ページをレンダリング
        pdf.RasterizeToImageFiles(
            Path.Combine(outputFolder, "page_*.png"),
            DPI: 150
        );
    }
}
```

### 例 2: テキストの抽出

**移行前 (MuPDF):**
```csharp
using MuPDFCore;
using System.Text;

public class MuPdfTextExtractor
{
    public string ExtractText(string pdfPath)
    {
        var sb = new StringBuilder();

        using (var context = new MuPDFContext())
        using (var document = new MuPDFDocument(context, pdfPath))
        {
            for (int i = 0; i < document.Pages.Count; i++)
            {
                using (var page = document.Pages[i])
                {
                    var text = page.GetText();
                    sb.AppendLine($"--- Page {i + 1} ---");
                    sb.AppendLine(text);
                }
            }
        }

        return sb.ToString();
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public class PdfTextExtractor
{
    public string ExtractText(string pdfPath)
    {
        var pdf = PdfDocument.FromFile(pdfPath);
        return pdf.ExtractAllText();
    }

    public string ExtractTextByPage(string pdfPath)
    {
        var pdf = PdfDocument.FromFile(pdfPath);
        var sb = new StringBuilder();

        for (int i = 0; i < pdf.PageCount; i++)
        {
            sb.AppendLine($"--- Page {i + 1} ---");
            sb.AppendLine(pdf.ExtractTextFromPage(i));
        }

        return sb.ToString();
    }
}
```

### 例 3: PDF 作成 (MuPDF では不可能)

**移行前 (MuPDF):**
```csharp
// MuPDF は HTML から PDF を作成できません
// 外部ツール（例: wkhtmltopdf）を使用し、MuPDF で表示用に読み込む必要があります
throw new NotSupportedException("MuPDF はレンダラーであり、PDF 作成者ではありません");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public class PdfCreator
{
    private readonly ChromePdfRenderer _renderer;

    public PdfCreator()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
        _renderer = new ChromePdfRenderer();

        // レンダリングオプションの設定
        _renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        _renderer.RenderingOptions.MarginTop = 20;
        _renderer.RenderingOptions.MarginBottom = 20;
    }

    public void CreateFromHtml(string html, string outputPath)
    {
        var pdf = _renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs(outputPath);
    }

    public void CreateFromUrl(string url, string outputPath)
    {
        var pdf = _renderer.RenderUrlAsPdf(url);
        pdf.SaveAs(outputPath);
    }
}
```

### 例 4: PDF のマージ (MuPDF では複雑)

**移行前 (MuPDF):**
```csharp
using MuPDFCore;

// MuPDF は限定的なマージサポートを提供
// 通常、ページを一つずつコピーする必要があります
public class MuPdfMerger
{
    public void MergePdfs(string[] inputPaths, string outputPath)
    {
        using (var context = new MuPDFContext())
        using (var outputDoc = MuPDFDocument.Create())
        {
            foreach (var inputPath in inputPaths)
            {
                using (var inputDoc = new MuPDFDocument(context, inputPath))
                {
                    for (int i = 0; i < inputDoc.Pages.Count; i++)
                    {
                        // 複雑なページコピーロジック
                        outputDoc.CopyPage(inputDoc, i);
                    }
                }
            }

            outputDoc.Save(outputPath);
        }
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;
using System.Linq;

public class PdfMerger
{
    public void MergePdfs(string[] inputPaths, string outputPath)
    {
        var pdfs = inputPaths.Select(PdfDocument.FromFile).ToList();
        var merged = PdfDocument.Merge(pdfs);
        merged.SaveAs(outputPath);
    }
}
```

### 例 5: ページ情報と寸法

**移行前 (MuPDF):**
```csharp
using MuPDFCore;
using System;

public class MuPdfPageInfo
{
    public void PrintPageInfo(string pdfPath)
    {
        using (var context = new MuPDFContext())
        using (var document = new MuPDFDocument(context, pdfPath))
        {
            Console.WriteLine($"Total pages: {document.Pages.Count}");

            for (int i = 0; i < document.Pages.Count; i++)
            {
                using (var page = document.Pages[i])
                {
                    Console.WriteLine($"Page {i + 1}: {page.Width}x{page.Height}");
                }
            }
        }
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;
using System;

public class PdfPageInfo
{
    public void PrintPageInfo(string pdfPath)
    {
        var pdf = PdfDocument.FromFile(pdfPath);

        Console.WriteLine($"Total pages: {pdf.PageCount}");

        foreach (var page in pdf.Pages)
        {
            Console.WriteLine($"Page {page.PageIndex + 1}: {page.Width}x{page.Height}");
        }
    }
}
```

### 例 6: 特定のページのレンダリング

**移行前 (MuPDF):**
```csharp
using MuPDFCore;

public class MuPdfPageRenderer
{
    public byte[] RenderPageTo