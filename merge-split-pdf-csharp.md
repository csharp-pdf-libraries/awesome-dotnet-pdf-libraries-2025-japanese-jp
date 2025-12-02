---
** ** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/merge-split-pdf-csharp.md)

---
# C#でPDFをマージおよび分割する：ライブラリ比較完全ガイド

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFのクリエーター

[![.NET](https://img.shields.io/badge/.NET-6%2B-512BD4)](https://dotnet.microsoft.com/)
[![最終更新](https://img.shields.io/badge/Updated-November%202025-green)]()

> PDFのマージと分割は、最も一般的なPDF操作です。このガイドでは、異なるC#ライブラリがこれらのタスクをどのように扱うか、そしてどれが最も優れているかを比較します。

---

## 目次

1. [クイックスタート](#quick-start)
2. [ライブラリ比較](#library-comparison)
3. [PDFのマージ](#merging-pdfs)
4. [PDFの分割](#splitting-pdfs)
5. [高度な操作](#advanced-operations)
6. [パフォーマンス比較](#performance-comparison)
7. [推奨事項](#recommendations)

---

## クイックスタート

### IronPDFでPDFをマージする（最も簡単）

```csharp
using IronPdf;

// 複数のPDFをマージ
var merged = PdfDocument.Merge("doc1.pdf", "doc2.pdf", "doc3.pdf");
merged.SaveAs("combined.pdf");
```

### IronPDFでPDFを分割する

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// 特定のページを抽出
var pages1to3 = pdf.CopyPages(0, 2);
pages1to3.SaveAs("first-three-pages.pdf");

// 残りのページを抽出
var remaining = pdf.CopyPages(3, pdf.PageCount - 1);
remaining.SaveAs("remaining-pages.pdf");
```

---

## ライブラリ比較

### マージ/分割機能マトリックス

| ライブラリ | PDFをマージ | PDFを分割 | ページを抽出 | ページを挿入 | ページを削除 |
|---------|-----------|-----------|--------------|--------------|--------------|
| **IronPDF** | ✅ 1行 | ✅ 1行 | ✅ | ✅ | ✅ |
| iText7 | ✅ 10+行 | ✅ 10+行 | ✅ | ✅ | ✅ |
| Aspose.PDF | ✅ 5+行 | ✅ 5+行 | ✅ | ✅ | ✅ |
| PDFSharp | ✅ 15+行 | ✅ 15+行 | ✅ | ✅ | ⚠️ |
| PuppeteerSharp | ❌ | ❌ | ❌ | ❌ | ❌ |
| QuestPDF | ❌ | ❌ | ❌ | ❌ | ❌ |
| wkhtmltopdf | ❌ | ❌ | ❌ | ❌ | ❌ |

**主な洞察:** 生成専用ライブラリ（PuppeteerSharp、QuestPDF、wkhtmltopdf）は既存のPDFを操作できません。

### コードの複雑さ比較

**IronPDF — 1行:**
```csharp
var merged = PdfDocument.Merge("a.pdf", "b.pdf");
```

**iText7 — 15+行:**
```csharp
using iText.Kernel.Pdf;
using iText.Kernel.Utils;

using var destPdf = new PdfDocument(new PdfWriter("merged.pdf"));
var merger = new PdfMerger(destPdf);

using var pdf1 = new PdfDocument(new PdfReader("a.pdf"));
using var pdf2 = new PdfDocument(new PdfReader("b.pdf"));

merger.Merge(pdf1, 1, pdf1.GetNumberOfPages());
merger.Merge(pdf2, 1, pdf2.GetNumberOfPages());

destPdf.Close();
```

**PDFSharp — 20+行:**
```csharp
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

var outputDoc = new PdfDocument();

using var doc1 = PdfReader.Open("a.pdf", PdfDocumentOpenMode.Import);
using var doc2 = PdfReader.Open("b.pdf", PdfDocumentOpenMode.Import);

foreach (var page in doc1.Pages)
    outputDoc.AddPage(page);

foreach (var page in doc2.Pages)
    outputDoc.AddPage(page);

outputDoc.Save("merged.pdf");
```

---

## PDFのマージ

### 基本的なマージ

```csharp
using IronPdf;

// 方法1: ファイルパスから
var merged = PdfDocument.Merge("invoice1.pdf", "invoice2.pdf", "invoice3.pdf");
merged.SaveAs("all-invoices.pdf");

// 方法2: PdfDocumentオブジェクトから
var doc1 = PdfDocument.FromFile("doc1.pdf");
var doc2 = PdfDocument.FromFile("doc2.pdf");
var combined = PdfDocument.Merge(doc1, doc2);
combined.SaveAs("combined.pdf");

// 方法3: バイト配列から
byte[] pdf1Bytes = File.ReadAllBytes("doc1.pdf");
byte[] pdf2Bytes = File.ReadAllBytes("doc2.pdf");
var merged = PdfDocument.Merge(
    new PdfDocument(pdf1Bytes),
    new PdfDocument(pdf2Bytes)
);
```

### ディレクトリからマージ

```csharp
using IronPdf;

// ディレクトリ内のすべてのPDFを取得
var pdfFiles = Directory.GetFiles("invoices/", "*.pdf")
                        .OrderBy(f => f)
                        .ToArray();

// すべてをマージ
var merged = PdfDocument.Merge(pdfFiles);
merged.SaveAs("all-invoices.pdf");

Console.WriteLine($"Merged {pdfFiles.Length} files into one PDF");
```

### 目次付きでマージ

```csharp
using IronPdf;

var pdf1 = PdfDocument.FromFile("chapter1.pdf");
var pdf2 = PdfDocument.FromFile("chapter2.pdf");
var pdf3 = PdfDocument.FromFile("chapter3.pdf");

// 目次を作成
var tocHtml = @"
<h1>Table of Contents</h1>
<ul>
    <li><a href='#chapter1'>Chapter 1: Introduction</a></li>
    <li><a href='#chapter2'>Chapter 2: Implementation</a></li>
    <li><a href='#chapter3'>Chapter 3: Conclusion</a></li>
</ul>";

var toc = ChromePdfRenderer.RenderHtmlAsPdf(tocHtml);

// 目次を最初にマージ
var book = PdfDocument.Merge(toc, pdf1, pdf2, pdf3);
book.SaveAs("complete-book.pdf");
```

### 異なるページサイズでマージ

```csharp
using IronPdf;

// IronPDFは自動的に異なるページサイズを処理
var a4Doc = PdfDocument.FromFile("a4-report.pdf");
var letterDoc = PdfDocument.FromFile("letter-appendix.pdf");

var merged = PdfDocument.Merge(a4Doc, letterDoc);
merged.SaveAs("mixed-sizes.pdf");  // 各ページは元のサイズを保持
```

---

## PDFの分割

### 個々のページに分割

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("large-document.pdf");

for (int i = 0; i < pdf.PageCount; i++)
{
    var singlePage = pdf.CopyPage(i);
    singlePage.SaveAs($"page-{i + 1}.pdf");
}

Console.WriteLine($"Split into {pdf.PageCount} individual files");
```

### ページ範囲による分割

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("annual-report.pdf");

// 第1セクション: ページ1-10
var intro = pdf.CopyPages(0, 9);
intro.SaveAs("01-introduction.pdf");

// 第2セクション: ページ11-30
var mainContent = pdf.CopyPages(10, 29);
mainContent.SaveAs("02-main-content.pdf");

// 第3セクション: ページ31から終わりまで
var appendix = pdf.CopyPages(30, pdf.PageCount - 1);
appendix.SaveAs("03-appendix.pdf");
```

### ブックマークを使用して章ごとに分割

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("book.pdf");
var bookmarks = pdf.BookMarks;

for (int i = 0; i < bookmarks.Count; i++)
{
    int startPage = bookmarks[i].PageIndex;
    int endPage = (i + 1 < bookmarks.Count)
        ? bookmarks[i + 1].PageIndex - 1
        : pdf.PageCount - 1;

    var chapter = pdf.CopyPages(startPage, endPage);
    chapter.SaveAs($"chapter-{i + 1}-{bookmarks[i].Text}.pdf");
}
```

### メール送信用に大きなPDFを分割

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("large-report.pdf");
const int maxPagesPerFile = 10;
const int maxSizeBytes = 5 * 1024 * 1024; // 5MB

int part = 1;
int startPage = 0;

while (startPage < pdf.PageCount)
{
    int endPage = Math.Min(startPage + maxPagesPerFile - 1, pdf.PageCount - 1);
    var chunk = pdf.CopyPages(startPage, endPage);

    // サイズをチェックし、必要に応じてページ数を減らす
    while (chunk.BinaryData.Length > maxSizeBytes && endPage > startPage)
    {
        endPage--;
        chunk = pdf.CopyPages(startPage, endPage);
    }

    chunk.SaveAs($"report-part{part:D2}.pdf");
    Console.WriteLine($"Part {part}: pages {startPage + 1}-{endPage + 1}, {chunk.BinaryData.Length / 1024}KB");

    startPage = endPage + 1;
    part++;
}
```

---

## 高度な操作

### ページを挿入

```csharp
using IronPdf;

var mainDoc = PdfDocument.FromFile("main-document.pdf");
var insertDoc = PdfDocument.FromFile("insert-page.pdf");

// 特定の位置に挿入（5ページ目の後）
mainDoc.InsertPdf(insertDoc, 5);
mainDoc.SaveAs("with-insert.pdf");
```

### ページを削除

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// 単一ページを削除（0から始まるインデックス）
pdf.RemovePage(2);  // 3ページ目を削除

// 複数ページを削除
pdf.RemovePages(new[] { 0, 5, 10 });  // 1, 6, 11ページを削除

pdf.SaveAs("cleaned.pdf");
```

### ページの並び替え

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// 新しい順序を作成：最後のページを最初に移動
var newOrder = new List<int> { pdf.PageCount - 1 };
newOrder.AddRange(Enumerable.Range(0, pdf.PageCount - 1));

var reordered = pdf.CopyPages(newOrder.ToArray());
reordered.SaveAs("reordered.pdf");
```

### 奇数/偶数ページを抽出

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// 奇数ページを抽出（1, 3, 5, ...）
var oddPages = Enumerable.Range(0, pdf.PageCount).Where(i => i % 2 == 0).ToArray();
var oddDoc = pdf.CopyPages(oddPages);
oddDoc.SaveAs("odd-pages.pdf");

// 偶数ページを抽出（2, 4, 6, ...）
var evenPages = Enumerable.Range(0, pdf.PageCount).Where(i => i % 2 == 1).ToArray();
var evenDoc = pdf.CopyPages(evenPages);
evenDoc.SaveAs("even-pages.pdf");
```

---

## パフォーマンス比較

### ベンチマーク：100個のPDFをマージ（各10ページ）

| ライブラリ | 時間 | メモリピーク |
|---------|------|-------------|
| **IronPDF** | 2.3秒 | 180MB |
| iText7 | 3.1秒 | 220MB |
| Aspose.PDF | 4.2秒 | 350MB |
| PDFSharp | 5.8秒 | 280MB |

### ベンチマーク：1000ページのPDFを分割

| ライブラリ | 時間 | メモリピーク |
|---------|------|-------------|
| **IronPDF** | 1.8秒 | 150MB |
| iText7 | 2.5秒 | 200MB |
| Aspose.PDF | 3.9秒 | 320MB |
| PDFSharp | 4.5秒 | 250MB |

*2025年11月、Azure D2s v3で実施されたテスト*

---

## 推奨事項

### IronPDFを選ぶべき時:
- ✅ 単純な一行操作が必要な場合
- ✅ HTMLからPDFへの生成も必要な場合
- ✅ クロスプラットフォーム展開（Windows/Linux/Docker）が必要な場合
- ✅ APIのシンプルさを重視する場合

### iText7を選ぶべき時:
- 他の操作でiTextを既に使用している場合
- 非常に具体的な低レベル制御が必要な場合
- AGPLライセンスが許容できる場合（または商用ライセンスを購入する場合）

### PDFSharpを選ぶべき時:
- 予算がゼロの場合
- 基本的なマージ/分割のみが必要な場合
- より冗長なAPIを受け入れることができる場合

### マージ/分割には避けるべき:
- **PuppeteerSharp** — 生成のみ
- **QuestPDF** — 生成のみ
- **wkhtmltopdf** — 生成のみ

---

## 結論

C#でのPDFマージおよび分割操作について:

1. **最もシンプルなAPI:** IronPDF（ほとんどの操作で1行）
2. **無料オプション:** PDFSharp（しかしコードが冗長）
3. **最もコントロールが可能:** iText7（しかしAGPLまたは高価）

IronPDFのマージ/分割操作は、他のライブラリで15〜20行かかる操作がIronPDFでは1〜2行で済むように、開発者の生産性を考慮して設計されています。

---

## 関連チュートリアル

- **[C#でHTMLからPDFへ](html-to-pdf-csharp.md)** — HTMLからPDFを生成
- **[PDFテキスト抽出](extract-text-from-pdf-csharp.md)** — PDFからコンテンツを抽出
- **[デジタル署名](digital-signatures-pdf-csharp.md)** — マージしたドキュメントに署名
- **[PDFへの透かし追加](watermark-pdf-csharp.md)** — 分割したドキュメントに透かしを追加

---

### その他のチュートリアル
- **[HTMLからPDFへ](html-to-pdf-csharp.md)** — マージするためのPDFを生成
- **[2025年のベストPDFライブラリ](best-pdf-libraries-dotnet-2025.md)** — 完全な比較
- **[クロスプラットフォーム展開](cross-platform-pdf-dotnet.md)** — マージ操作を展開
- **[IronPDFガイド](ironpdf/)** — ライブラリのドキュメント
- **[PDFSharpガイド](pdfsharp/)** — 代替アプローチ
- **[iTextガイド](itext-itextsharp/)** — 複雑なマージ操作

---

*「[Awesome .NET PDF Libraries 2025](README.md)」コレクションの一部 — 73のC#/.NET PDFライブラリを比較*