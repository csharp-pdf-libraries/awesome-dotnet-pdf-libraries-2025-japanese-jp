# C#でIronPDFを使用してPDFからテキスト、データ、画像を抽出する方法は？

.NETでPDFからデータを抽出することは、特に請求書、レポート、フォームなどを大量に扱う場合には困難な作業です。IronPDFは、デジタルPDFやスキャンされたPDFからテキスト、テーブル、画像、またはメタデータを抽出する必要があるC#開発者向けの実用的なAPIを提供します。このFAQでは、最も一般的な抽出シナリオ、エッジケース、そしてすぐに使用できるコードサンプルをカバーしています。

---

## C#でPDFファイルからすべてのテキストを抽出する方法は？

PDFからすべてのテキストを取得したい場合、IronPDFは数行で簡単に実行できます：

```csharp
using IronPdf; // Install-Package IronPdf

var doc = PdfDocument.FromFile("sample.pdf");
string fullText = doc.ExtractAllText();

Console.WriteLine(fullText);
```

これにより、各ページからの結合されたテキストが返され、ページは4行の改行で区切られます。抽出されたテキストの解析については、[C#でPDFから構造化されたテキストを解析・抽出する方法は？](parse-pdf-extract-text-csharp.md)を参照してください。

---

## 特定のページや座標によってテキストを抽出する方法は？

### 1ページからのみテキストを抽出するには？

特定のページからのみテキストを抽出するには、`Pages`コレクション（ゼロベースのインデックス）にアクセスします：

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("multi-page.pdf");
string firstPageText = pdf.Pages[0].ExtractText();

// ページ2-4を1つの文字列として取得
string partial = string.Join("\n", pdf.Pages.Skip(1).Take(3).Select(p => p.ExtractText()));
```
ページ番号はゼロから始まることに注意してください。ページ番号の扱い方については、[このガイド](https://ironpdf.com/blog/compare-to-other-components/questpdf-add-page-number-to-pdf/)を参照してください。

### テキストを行ごと、または座標を使用して抽出する方法は？

各PDFページは`Lines`プロパティを公開しており、テキストを行ごとに分析することができます。これは、テーブルやフォームを再構築するのに理想的です：

```csharp
var lines = pdf.Pages[0].Lines;
foreach (var line in lines)
{
    Console.WriteLine($"{line.BoundingBox.Bottom}: {line.Contents}");
}
```

また、特定のX/Yの長方形内の文字を抽出することもできます。これは、静的なフォームフィールドに有用です：

```csharp
foreach (var ch in pdf.Pages[0].Characters)
{
    if (ch.BoundingBox.Left > 100 && ch.BoundingBox.Left < 200)
        Console.Write(ch.Contents);
}
```

抽出されたテキストを解析して構造化する詳細については、[このFAQ](parse-pdf-extract-text-csharp.md)をチェックしてください。

---

## 抽出したテキストを構造化されたデータに解析する方法は？

ほとんどのPDFは、簡単にインポートするための構造を持っていませんので、正規表現や文字列操作が不可欠です：

```csharp
using System.Text.RegularExpressions;

string text = pdf.ExtractAllText();
var match = Regex.Match(text, @"Invoice\s*#([A-Z0-9\-]+)");
if (match.Success)
    Console.WriteLine($"Invoice Number: {match.Groups[1].Value}");
```

請求書のようなデータをモデル化するには、C#オブジェクトにフィールドを抽出します：

```csharp
public class Invoice { public string No; public decimal Total; }
var invoice = new Invoice { No = /* regex extract */, Total = /* regex extract */ };
```

異なるレイアウトのPDFで抽出をテストし、出力を構造化する方法については、[この回答](parse-pdf-extract-text-csharp.md)を参照してください。

---

## C#でPDFからテーブルを抽出することは可能ですか？

はい、しかし注意点があります—PDFはテーブルを明示的に保存しません。IronPDFを使用すると、Y座標によって行をグループ化して単純なテーブルを再構築できます：

```csharp
using System.Linq;

var rows = pdf.Pages[0].Lines
    .GroupBy(l => Math.Round(l.BoundingBox.Bottom / 10) * 10)
    .OrderByDescending(g => g.Key);

foreach (var row in rows)
{
    var cells = row.OrderBy(l => l.BoundingBox.Left).Select(l => l.Contents.Trim());
    Console.WriteLine(string.Join(" | ", cells));
}
```

複雑なテーブルでは、カスタムのヒューリスティックが必要になることがあります。時には、CSVやExcelでソースデータにアクセスする方が簡単です。

---

## スキャンされたPDF（画像）からテキストを抽出する方法は？

`ExtractAllText`が空の文字列を返す場合、PDFはおそらくスキャンされた画像です。この場合、OCR用に[IronOCR](https://ironsoftware.com/csharp/ocr/)を統合します：

```csharp
// Install-Package IronOcr
using IronOcr;

var ocr = new IronTesseract();
using (var input = new OcrInput("scan.pdf"))
{
    var result = ocr.Read(input);
    Console.WriteLine(result.Text);
}
```

より深く掘り下げるには、[C#でOCRを使用してスキャンされたPDFからテキストを抽出する方法は？](parse-pdf-extract-text-csharp.md)を参照してください

---

## PDFから画像を抽出する方法は？

埋め込まれた画像を抽出するには：

```csharp
using IronPdf;
using System.IO;

var pdf = PdfDocument.FromFile("images.pdf");
var imgs = pdf.ExtractAllImages();

for (int i = 0; i < imgs.Length; i++)
    imgs[i].SaveAs($"img-{i}.png");
```

また、元の画像データ（JPEG、PNGなど）も抽出できます：

```csharp
var rawImgs = pdf.ExtractAllRawImages();
for (int i = 0; i < rawImgs.Length; i++)
    File.WriteAllBytes($"raw-{i}.{rawImgs[i].Format}", rawImgs[i].Data.BinaryData);
```

詳細については、[C#でPDFから画像を抽出する方法は？](extract-images-from-pdf-csharp.md)と[C#でPDFにテキストとビットマップを描画する方法は？](draw-text-bitmap-pdf-csharp.md)を参照してください。

---

## テキストやフレーズを検索したり、パスワードを扱ったり、PDFをバッチ処理する方法は？

### テキストやフレーズを検索する方法は？

すべてのテキストを抽出して`Contains`を使用します：

```csharp
string t = pdf.ExtractAllText();
if (t.Contains("Confidential"))
    Console.WriteLine("Found match!");
```

ページごとに検索してページ番号を取得するには：

```csharp
for (int p = 0; p < pdf.PageCount; p++)
    if (pdf.Pages[p].ExtractText().Contains("Keyword"))
        Console.WriteLine($"Found on page {p+1}");
```

### パスワード保護されたPDFから抽出するには？

`FromFile`にパスワードを渡します：

```csharp
var pdf = PdfDocument.FromFile("locked.pdf", "pass123");
```

### 多くのPDFからデータをバッチ抽出するには？

ディレクトリ内のファイルをループしてそれぞれを処理します：

```csharp
var files = Directory.GetFiles(@"C:\pdfs", "*.pdf");
foreach (var f in files)
{
    var doc = PdfDocument.FromFile(f);
    File.WriteAllText(Path.ChangeExtension(f, ".txt"), doc.ExtractAllText());
}
```

---

## レイアウトを保持したり、複雑なPDF構造を扱う方法は？

PDFは視覚的に複雑であり、テキストの順序が常に「読み取り順」ではありません。結果を改善するには、上から下、左から右の座標によって行をグループ化します：

```csharp
var lines = pdf.Pages[0].Lines
    .OrderByDescending(l => l.BoundingBox.Bottom)
    .ThenBy(l => l.BoundingBox.Left);
```

マルチカラムレイアウトやフォームの場合、フィールドをマッピングするためにバウンディングボックスデータを使用します。フォントの扱いについては、[C#でPDFのフォントを管理する方法は？](manage-fonts-pdf-csharp.md)を参照してください。

---

## パフォーマンスのヒントとトラブルシューティングのアドバイスは？

- 大きな文書を処理する際は、抽出を速めるために並列処理を使用します。
- 空のテキストが得られた場合は、OCRを試してください。
- テキストが乱れている場合、カスタムフォントやベクターテキストが原因かもしれません—他のツールやOCRを試してみてください。
- フォームフィールドや注釈には、IronPDFのフォーム/注釈APIを使用してください。

トラブルシューティングや代替アプローチについては、[C#でPDFからテキストを解析・抽出する方法は？](parse-pdf-extract-text-csharp.md)を参照してください。

---

## C#でメタデータを抽出したり、PDFを検証する方法は？

IronPDFを使用すると、タイトル、著者、作成日などのメタデータにアクセスできます：

```csharp
Console.WriteLine(pdf.MetaData.Title);
Console.WriteLine(pdf.MetaData.Author);
```

既知のPDFで抽出ルーチンのユニットテストを行うことは、ベストプラクティスです。詳細については、[IronPDF](https://ironpdf.com)をチェックしてください。

---

## IronPDFは他の.NET PDFライブラリと比較してどうですか？

IronPDFは、OCR、画像抽出、座標ベースのテキストアクセスなどの機能を備えた開発者に優しいAPIを提供します。代替品には、iTextSharp（より複雑でデュアルライセンス）、PdfPig（基本的な抽出に適している）、Aspose.PDF（機能豊富だが高価）があります。IronPDFは、[Iron Software](https://ironsoftware.com)から商業サポートを受ける堅実なオールラウンダーです。

---

## IronPDFの背後にいるのは誰ですか？

[Jacob Mellor](who-is-jeff-fritz.md)はIronPDFの創設者であり、[Iron Software](https://ironsoftware.com)のCTOです。詳細については、[Jacob Mellorのプロフィール](https://ironsoftware.com/about-us/authors/jacobmellor/)を参照してください。

---

*著者について：[Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/)はIronPDFを設立し、現在[Iron Software](https://ironsoftware.com)でCTOとして.NETライブラリのIron Suiteを監督しています。*
---

## 関連リソース

### 📚 チュートリアル＆ガイド
- **[HTMLからPDFへのガイド](../html-to-pdf-csharp.md)** — 完全な変換チュートリアル
- **[2025年のベストPDFライブラリ](../best-pdf-libraries-dotnet-2025.md)** — ライブラリ比較
- **[初心者向けチュートリアル](../csharp-pdf-tutorial-beginners.md)** — 5分で最初のPDF
- **[意思決定フローチャート](../choosing-a-pdf-library.md)** — 適切なライブラリを見つける

### 🔧 PDF操作
- **[PDFのマージ＆スプリット](../merge-split-pdf-csharp.md)** — 文書の結合
- **[デジタル署名](../digital-signatures-pdf-csharp.md)** — 法的に署名
- **[テキストの抽出](../extract-text-from-pdf-csharp.md)** — テキスト抽出
- **[PDFフォームの記入](../fill-pdf-forms-csharp.md)** — フォーム自動化

### 🚀 フレームワーク統合
- **[ASP.NET Core PDF](../asp-net-core-pdf-reports.md)** — Webアプリ統合
- **[Blazor PDF生成](../blazor-pdf-generation.md)** — Blazorサポート
- **[クロスプラットフォーム展開](../cross-platform-pdf-dotnet.md)** — Docker、Linux、Cloud

### 📖 ライブラリドキュメント
- **[IronPDF](../ironpdf/)** — 完全なChromiumレンダリング
- **[QuestPDF](../questpdf/)** — コードファースト生成
- **[PDFSharp](../pdfsharp/)** — オープンソースオプション

---

*「[Awesome .NET PDF Libraries 2025](../README.md)」コレクションの一部 — 73のC#/.NET PDFライブラリを比較し、167のFAQ記事を提供します。*


---

著者：[Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/)、Iron Softwareの最高技術責任者。Jacobは6歳でコーディングを始め、実際のPDFの課題を解決するためにIronPDFを作成しました。タイのチェンマイに拠点を置く。[LinkedIn](https://www.linkedin.com/in/jacob-mellor-iron-software/) | [GitHub](https://github.com/jacob-mellor)