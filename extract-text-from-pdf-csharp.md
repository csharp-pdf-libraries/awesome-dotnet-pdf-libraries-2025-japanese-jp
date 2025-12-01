---
**🌐 日本語版** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/extract-text-from-pdf-csharp.md)

---

# PDFからテキストを抽出するC#：OCRサポート付き完全ガイド

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFの作成者

[![.NET](https://img.shields.io/badge/.NET-6%2B-512BD4)](https://dotnet.microsoft.com/)
[![最終更新](https://img.shields.io/badge/Updated-11月%202025-green)]()

> PDFからテキストを抽出することは、検索インデックス作成、データ移行、コンテンツ分析に不可欠です。このガイドでは、テキストベースおよびスキャンされたPDFの両方をカバーし、ライブラリの比較を行います。

---

## 目次

1. [クイックスタート](#quick-start)
2. [ライブラリ比較](#library-comparison)
3. [全テキストを抽出](#extract-all-text)
4. [ページごとの抽出](#page-by-page-extraction)
5. [構造化抽出](#structured-extraction)
6. [スキャンされたPDFのOCR](#ocr-for-scanned-pdfs)
7. [パフォーマンス最適化](#performance-optimization)
8. [一般的な使用例](#common-use-cases)

---

## クイックスタート

### IronPDFで全テキストを抽出

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");
string allText = pdf.ExtractAllText();
Console.WriteLine(allText);
```

### 特定のページから抽出

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");
string pageText = pdf.ExtractTextFromPage(0);  // 最初のページ
Console.WriteLine(pageText);
```

---

## ライブラリ比較

### テキスト抽出機能

| ライブラリ | テキスト抽出 | ページごと | 構造化 | OCR | テーブル検出 |
|---------|-------------|---------|-----------|-----|-----------------|
| **IronPDF** | ✅ シンプル | ✅ | ✅ | ✅ (IronOCR) | ⚠️ |
| iText7 | ✅ | ✅ | ✅ | ❌ | ⚠️ |
| Aspose.PDF | ✅ | ✅ | ✅ | ❌ | ✅ |
| PDFSharp | ⚠️ 限定 | ⚠️ | ❌ | ❌ | ❌ |
| pdfpig | ✅ | ✅ | ✅ | ❌ | ✅ |
| PuppeteerSharp | ❌ | ❌ | ❌ | ❌ | ❌ |
| QuestPDF | ❌ | ❌ | ❌ | ❌ | ❌ |

**重要な洞察:** 生成専用のライブラリではテキストを抽出できません。

### コードの複雑さ

**IronPDF — 2行:**
```csharp
var pdf = PdfDocument.FromFile("document.pdf");
string text = pdf.ExtractAllText();
```

**iText7 — 10+行:**
```csharp
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

using var reader = new PdfReader("document.pdf");
using var pdfDoc = new PdfDocument(reader);

var text = new StringBuilder();
for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
{
    text.AppendLine(PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i)));
}
```

---

## 全テキストを抽出

### 基本的な抽出

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("report.pdf");

// 文書全体から全テキストを取得
string allText = pdf.ExtractAllText();

// ファイルに保存
File.WriteAllText("extracted-text.txt", allText);
Console.WriteLine($"抽出された文字数 {allText.Length}");
```

### URLから

```csharp
using IronPdf;

// ダウンロードして抽出
var pdf = PdfDocument.FromFile(new Uri("https://example.com/document.pdf"));
string text = pdf.ExtractAllText();
```

### バイト配列から

```csharp
using IronPdf;

byte[] pdfBytes = await httpClient.GetByteArrayAsync("https://example.com/doc.pdf");
var pdf = new PdfDocument(pdfBytes);
string text = pdf.ExtractAllText();
```

---

## ページごとの抽出

### 単一ページ

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("book.pdf");

// 最初のページを抽出 (0から始まるインデックス)
string firstPage = pdf.ExtractTextFromPage(0);

// 最後のページを抽出
string lastPage = pdf.ExtractTextFromPage(pdf.PageCount - 1);
```

### ページ範囲

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("report.pdf");
var pageTexts = new List<string>();

// 5-10ページを抽出
for (int i = 4; i < 10 && i < pdf.PageCount; i++)
{
    string pageText = pdf.ExtractTextFromPage(i);
    pageTexts.Add($"=== ページ {i + 1} ===\n{pageText}");
}

string combined = string.Join("\n\n", pageTexts);
```

### 各ページを処理

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

for (int i = 0; i < pdf.PageCount; i++)
{
    string pageText = pdf.ExtractTextFromPage(i);

    // 各ページを処理
    int wordCount = pageText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    Console.WriteLine($"ページ {i + 1}: {wordCount} 単語");
}
```

---

## 構造化抽出

### レイアウト付きで抽出

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("formatted-document.pdf");

// ExtractAllTextはいくつかのレイアウト情報を保持
string text = pdf.ExtractAllText();

// 行は通常保持されます
string[] lines = text.Split('\n');
foreach (string line in lines)
{
    if (!string.IsNullOrWhiteSpace(line))
    {
        Console.WriteLine(line.Trim());
    }
}
```

### テーブルを抽出（基本）

```csharp
using IronPdf;
using System.Text.RegularExpressions;

var pdf = PdfDocument.FromFile("report-with-tables.pdf");
string text = pdf.ExtractAllText();

// 空白パターンによる基本的なテーブル検出
var lines = text.Split('\n');
var tableRows = new List<string[]>();

foreach (string line in lines)
{
    // 複数の空白区切りによる表形式データを検出
    if (Regex.IsMatch(line, @"\s{2,}"))
    {
        var cells = Regex.Split(line.Trim(), @"\s{2,}");
        if (cells.Length > 1)
        {
            tableRows.Add(cells);
        }
    }
}

// CSVとして出力
foreach (var row in tableRows)
{
    Console.WriteLine(string.Join(",", row.Select(c => $"\"{c}\"")));
}
```

### 見出しを抽出

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");
string text = pdf.ExtractAllText();

// 見出しのように見える行を探す（すべて大文字、短い、内容に続く）
var lines = text.Split('\n');
var headings = new List<string>();

for (int i = 0; i < lines.Length - 1; i++)
{
    string line = lines[i].Trim();

    // ヒューリスティック：すべて大文字またはタイトルケースの短い行
    if (line.Length > 0 && line.Length < 100)
    {
        if (line == line.ToUpper() || char.IsUpper(line[0]))
        {
            // 内容に続くかチェック
            string nextLine = lines[i + 1].Trim();
            if (nextLine.Length > line.Length)
            {
                headings.Add(line);
            }
        }
    }
}

Console.WriteLine("見出しを発見:");
headings.ForEach(h => Console.WriteLine($"  - {h}"));
```

---

## スキャンされたPDFのOCR

スキャンされた文書や画像ベースのPDFには、OCRが必要です。

### IronPDFとIronOCRを使用

```csharp
using IronOcr;
using IronPdf;

// まず、PDFページを画像に変換
var pdf = PdfDocument.FromFile("scanned-document.pdf");

// テキスト認識にIronOCRを使用
var ocr = new IronTesseract();
var ocrInput = new OcrInput();

// OCR入力にPDFを直接追加
ocrInput.AddPdf("scanned-document.pdf");

// OCRを実行
var result = ocr.Read(ocrInput);
Console.WriteLine(result.Text);

// またはページごとのテキストを取得
foreach (var page in result.Pages)
{
    Console.WriteLine($"ページ {page.PageNumber}: {page.Text}");
}
```

### OCRが必要かどうかを検出

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("unknown-document.pdf");
string extractedText = pdf.ExtractAllText();

// 抽出されたテキストが非常に少ない場合、スキャンされた/画像ベースの可能性が高い
bool needsOcr = extractedText.Trim().Length < 100;

if (needsOcr)
{
    Console.WriteLine("このPDFはスキャンされたものと思われます。OCRが必要です。");
    // 抽出にIronOCRを使用
}
else
{
    Console.WriteLine("テキストベースのPDF。直接抽出が成功しました。");
    Console.WriteLine(extractedText);
}
```

---

## パフォーマンス最適化

### 大規模な文書

```csharp
using IronPdf;

// 非常に大きな文書の場合、チャンクで処理
var pdf = PdfDocument.FromFile("large-document.pdf");

const int chunkSize = 50;
var allText = new StringBuilder();

for (int start = 0; start < pdf.PageCount; start += chunkSize)
{
    int end = Math.Min(start + chunkSize, pdf.PageCount);

    for (int i = start; i < end; i++)
    {
        allText.AppendLine(pdf.ExtractTextFromPage(i));
    }

    // チャンクを処理またはディスクに書き込み
    Console.WriteLine($"処理されたページ {start + 1}-{end}");
}
```

### 並行抽出

```csharp
using IronPdf;
using System.Collections.Concurrent;

var pdf = PdfDocument.FromFile("document.pdf");
var pageTexts = new ConcurrentDictionary<int, string>();

Parallel.For(0, pdf.PageCount, i =>
{
    string text = pdf.ExtractTextFromPage(i);
    pageTexts[i] = text;
});

// 順番に結合
var orderedText = pageTexts
    .OrderBy(kvp => kvp.Key)
    .Select(kvp => kvp.Value);

string fullText = string.Join("\n", orderedText);
```

---

## 一般的な使用例

### 検索インデックス作成

```csharp
using IronPdf;

public class PdfIndexEntry
{
    public string FilePath { get; set; }
    public string Content { get; set; }
    public DateTime Indexed { get; set; }
}

public PdfIndexEntry IndexPdf(string filePath)
{
    var pdf = PdfDocument.FromFile(filePath);

    return new PdfIndexEntry
    {
        FilePath = filePath,
        Content = pdf.ExtractAllText(),
        Indexed = DateTime.UtcNow
    };
}

// ディレクトリ内のすべてのPDFをインデックス
var entries = Directory.GetFiles("documents/", "*.pdf")
    .Select(IndexPdf)
    .ToList();
```

### 請求書データの抽出

```csharp
using IronPdf;
using System.Text.RegularExpressions;

var pdf = PdfDocument.FromFile("invoice.pdf");
string text = pdf.ExtractAllText();

// 一般的な請求書フィールドを抽出
var invoiceNumber = Regex.Match(text, @"Invoice\s*#?\s*:?\s*(\w+)", RegexOptions.IgnoreCase);
var totalAmount = Regex.Match(text, @"Total:?\s*\$?([\d,]+\.?\d*)", RegexOptions.IgnoreCase);
var dateMatch = Regex.Match(text, @"Date:?\s*([\d/\-]+)", RegexOptions.IgnoreCase);

Console.WriteLine($"請求書: {invoiceNumber.Groups[1].Value}");
Console.WriteLine($"合計: ${totalAmount.Groups[1].Value}");
Console.WriteLine($"日付: {dateMatch.Groups[1].Value}");
```

### 履歴書の解析

```csharp
using IronPdf;
using System.Text.RegularExpressions;

var pdf = PdfDocument.FromFile("resume.pdf");
string text = pdf.ExtractAllText();

// メールを抽出
var email = Regex.Match(text, @"[\w\.-]+@[\w\.-]+\.\w+");

// 電話を抽出
var phone = Regex.Match(text, @"\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}");

// スキルを抽出（一般的なセクションを検索）
var skillsSection = Regex.Match(text, @"Skills:?\s*(.*?)(?=Experience|Education|$)",
    RegexOptions.IgnoreCase | RegexOptions.Singleline);

Console.WriteLine($"メール: {email.Value}");
Console.WriteLine($"電話: {phone.Value}");
Console.WriteLine($"スキル: {skillsSection.Groups[1].Value.Trim()}");
```

---

## 推奨事項

### テキスト抽出にIronPDFを選ぶ時:
- ✅ APIがシンプル（2行）が必要な場合
- ✅ PDFの生成/操作も行う場合
- ✅ スキャンされた文書にIronOCRを組み合わせる場合
- ✅ クロスプラットフォームサポートが必要な場合

### pdfpigを選ぶ時:
- 予算がゼロの場合
- 詳細な位置情報が必要な場合
- テーブル抽出が主な使用例の場合

### iText7を選ぶ時:
- 非常に特定のテキスト位置が必要な場合
- 他の操作でiTextを既に使用している場合

### テキストを抽出できない:
- ❌ PuppeteerSharp — 生成のみ
- ❌ QuestPDF — 生成のみ
- ❌ wkhtmltopdf — 生成のみ

---

## 関連チュートリアル

- **[HTMLからPDFへ](html-to-pdf-csharp.md)** — 検索可能なPDFを生成
- **[PDF/Aコンプライアンス](pdf-a-compliance-csharp.md)** — アクセス可能な、検索可能なPDFを作成
- **[PDFのマージ](merge-split-pdf-csharp.md)** — 抽出前に文書を結合
- **[最高のPDFライブラリ](best-pdf-libraries-dotnet-2025.md)** — ライブラリの完全な比較

---

### その他のチュートリアル
- **[PDFの赤塗り](pdf-redaction-csharp.md)** — 抽出された機密データを削除
- **[検索と置換](pdf-find-replace-csharp.md)** — PDF内のテキストを変更
- **[ASP.NET Core](asp-net-core-pdf-reports.md)** — Web抽出サービス
- **[IronPDFガイド](ironpdf/)** — 完全な抽出API
- **[Aspose.PDFガイド](asposepdf/)** — 代替抽出

---

*「[Awesome .NET PDF Libraries 2025](README.md)」コレクションの一部 — 73のC#/.NET PDFライブラリを比較*