---
**  (Japanese Translation)**

 **English:** [migradoc/migrate-from-migradoc.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/migradoc/migrate-from-migradoc.md)
 **:** [migradoc/migrate-from-migradoc.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/migradoc/migrate-from-migradoc.md)

---
# MigraDocからIronPDFへの移行方法は？

## 目次
1. [MigraDocから移行する理由](#migradocから移行する理由)
2. [開始する前に](#開始する前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## MigraDocから移行する理由

### MigraDocの課題

MigraDocはプログラムによるPDF生成に強力ですが、根本的な制限があります：

1. **HTMLサポートなし**：既存のHTML/CSSデザインを活用できず、ドキュメントを要素ごとに手動で構築する必要があります
2. **独自のドキュメントモデル**：`Document`、`Section`、`Paragraph`、`Table`、`Style` APIをマスターするための学習曲線が急です
3. **限定的なスタイリング**：CSSに比べてスタイリングオプションが少なく、現代のWebデザインに合わせるのが難しい
4. **冗長なコード**：単純なレイアウトでも多数のコード行が必要です
5. **JavaScriptなし**：動的コンテンツやインタラクティブ要素をレンダリングできません
6. **基本的なチャートのみ**：Webチャートライブラリに比べてチャート機能が限定的です
7. **.NET Coreサポートが限定的**：PDFSharp/MigraDoc 6.xは改善されましたが、以前のバージョンには制限がありました

### IronPDFの利点

| 特徴 | MigraDoc | IronPDF |
|---------|----------|---------|
| コンテンツ定義 | プログラム (Document/Section/Paragraph) | HTML/CSS |
| 学習曲線 | 急 (独自のDOM) | 簡単 (Webスキル) |
| スタイリング | 限定的なプロパティ | 完全なCSS3 |
| JavaScript | なし | 完全なChromium実行 |
| テーブル | 列/行の手動定義 | HTML `<table>` とCSS |
| チャート | 基本的なMigraDocチャート | 任意のJavaScriptチャートライブラリ |
| 画像 | 手動でのサイズ/位置設定 | 標準のHTML `<img>` |
| レスポンシブレイアウト | サポートされていない | Flexbox、Grid |
| モダンデザイン | 難しい | CSSで自然に |

### 移行の利点

- **既存のWebスキルを使用**：独自のドキュメントモデルの代わりにHTML/CSSを使用
- **無制限のスタイリング**：Flexbox、Grid、アニメーションを含む完全なCSS3
- **モダンなチャート**：Chart.js、D3、Highcharts、または任意のJSライブラリを使用
- **シンプルなコード**：複雑なレイアウトに対して80%少ないコード
- **テンプレートエンジン**：Razor、Scriban、または任意のテンプレートシステムを使用
- **Web/PDFデザインの一貫性**：WebとPDFで同じHTML

---

## 開始する前に

### 前提条件

1. **.NET環境**：.NET Framework 4.6.2+または.NET Core 3.1+ / .NET 5+
2. **NuGetアクセス**：NuGetパッケージをインストールできること
3. **IronPDFライセンス**：無料試用版または購入したライセンスキー

### インストール

```bash
# MigraDocパッケージを削除
dotnet remove package PdfSharp-MigraDoc
dotnet remove package PdfSharp-MigraDoc-GDI
dotnet remove package PDFsharp.MigraDoc.Standard

# IronPDFをインストール
dotnet add package IronPdf
```

### ライセンス設定

```csharp
// アプリケーションの起動時に追加 (Program.csまたはStartup.cs)
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### MigraDocの使用箇所を特定

```bash
# すべてのMigraDoc参照を検索
grep -r "using MigraDoc\|PdfDocumentRenderer\|AddSection\|AddParagraph" --include="*.cs" .
grep -r "AddTable\|AddRow\|AddColumn\|AddCell\|AddImage" --include="*.cs" .
```

---

## クイックスタート移行

### 最小変更例

**MigraDoc前：**
```csharp
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

public class MigraDocService
{
    public byte[] GenerateReport(ReportData data)
    {
        // ドキュメントを作成
        Document document = new Document();
        document.DefaultPageSetup.PageFormat = PageFormat.A4;

        // セクションを追加
        Section section = document.AddSection();

        // タイトルを追加
        Paragraph title = section.AddParagraph(data.Title);
        title.Format.Font.Size = 24;
        title.Format.Font.Bold = true;
        title.Format.SpaceAfter = 20;

        // テーブルを追加
        Table table = section.AddTable();
        table.Borders.Width = 0.75;

        Column col1 = table.AddColumn("5cm");
        Column col2 = table.AddColumn("3cm");

        Row headerRow = table.AddRow();
        headerRow.Shading.Color = Colors.LightGray;
        headerRow.Cells[0].AddParagraph("Name");
        headerRow.Cells[1].AddParagraph("Value");

        foreach (var item in data.Items)
        {
            Row row = table.AddRow();
            row.Cells[0].AddParagraph(item.Name);
            row.Cells[1].AddParagraph(item.Value.ToString());
        }

        // PDFにレンダリング
        PdfDocumentRenderer renderer = new PdfDocumentRenderer();
        renderer.Document = document;
        renderer.RenderDocument();

        using (MemoryStream ms = new MemoryStream())
        {
            renderer.PdfDocument.Save(ms);
            return ms.ToArray();
        }
    }
}
```

**IronPDF後：**
```csharp
using IronPdf;

public class PdfService
{
    private readonly ChromePdfRenderer _renderer;

    public PdfService()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
        _renderer = new ChromePdfRenderer();
        _renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    }

    public byte[] GenerateReport(ReportData data)
    {
        string html = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; padding: 20px; }}
                    h1 {{ font-size: 24px; font-weight: bold; margin-bottom: 20px; }}
                    table {{ border-collapse: collapse; width: 100%; }}
                    th, td {{ border: 1px solid #333; padding: 8px; }}
                    th {{ background-color: #d3d3d3; width: 5cm; }}
                    td:last-child {{ width: 3cm; }}
                </style>
            </head>
            <body>
                <h1>{data.Title}</h1>
                <table>
                    <tr><th>Name</th><th>Value</th></tr>
                    {string.Join("", data.Items.Select(i => $"<tr><td>{i.Name}</td><td>{i.Value}</td></tr>"))}
                </table>
            </body>
            </html>";

        return _renderer.RenderHtmlAsPdf(html).BinaryData;
    }
}
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| MigraDoc 名前空間 | IronPDF相当 | 備考 |
|-------------------|-------------------|-------|
| `MigraDoc.DocumentObjectModel` | `IronPdf` | メイン名前空間 |
| `MigraDoc.DocumentObjectModel.Tables` | HTMLテーブルを使用 | `<table>` |
| `MigraDoc.DocumentObjectModel.Shapes` | HTML/CSSを使用 | `<div>`、`<img>` |
| `MigraDoc.DocumentObjectModel.Shapes.Charts` | JSチャートを使用 | Chart.jsなど |
| `MigraDoc.Rendering` | `IronPdf` | レンダラー |
| `PdfSharp.Pdf` | `IronPdf` | PDF操作 |

### クラスマッピング

| MigraDocクラス | IronPDF相当 | 備考 |
|---------------|-------------------|-------|
| `Document` | `ChromePdfRenderer` | ドキュメントではなくレンダラーを使用 |
| `Section` | HTML `<body>` または `<div>` | 構造コンテナ |
| `Paragraph` | HTML `<p>`、`<h1>`など | テキスト要素 |
| `FormattedText` | HTML `<span>`、`<strong>`など | インラインフォーマット |
| `Table` | HTML `<table>` | CSSスタイリング付き |
| `Row` | HTML `<tr>` | テーブル行 |
| `Column` | HTML `<col>` または CSS | 列スタイリング |
| `Cell` | HTML `<td>`、`<th>` | テーブルセル |
| `Image` | HTML `<img>` | src属性付き |
| `TextFrame` | HTML `<div>` | CSSポジショニング付き |
| `Chart` | JSチャートライブラリ | Chart.js、D3など |
| `Style` | CSSクラスまたはインラインスタイル | 完全なCSSサポート |
| `HeadersFooters` | `RenderingOptions.HtmlHeader/Footer` | HTMLベース |
| `PageSetup` | `RenderingOptions.*` | ページ設定 |

### プロパティマッピング

| MigraDocプロパティ | IronPDF/CSS相当 | 備考 |
|------------------|------------------------|-------|
| `PageSetup.PageFormat = A4` | `RenderingOptions.PaperSize = PdfPaperSize.A4` | 用紙サイズ |
| `PageSetup.Orientation` | `RenderingOptions.PaperOrientation` | 縦/横 |
| `PageSetup.TopMargin` | `RenderingOptions.MarginTop` | mm単位 |
| `Format.Font.Size = 14` | CSS `font-size: 14pt` | フォントサイズ |
| `Format.Font.Bold = true` | CSS `font-weight: bold` | 太字テキスト |
| `Format.Font.Italic = true` | CSS `font-style: italic` | イタリックテキスト |
| `Format.Font.Color = Colors.Blue` | CSS `color: blue` | テキスト色 |
| `Format.Font.Name = "Arial"` | CSS `font-family: Arial` | フォントファミリー |
| `Format.Alignment = Center` | CSS `text-align: center` | テキストアラインメント |
| `Format.SpaceAfter = 10` | CSS `margin-bottom: 10pt` | スペーシング |
| `Format.SpaceBefore = 10` | CSS `margin-top: 10pt` | スペーシング |
| `Format.LeftIndent = 20` | CSS `margin-left: 20pt` | インデント |
| `Table.Borders.Width = 1` | CSS `border: 1px solid black` | テーブルボーダー |
| `Row.Shading.Color` | CSS `background-color` | 行の背景 |
| `Cell.VerticalAlignment` | CSS `vertical-align` | セルアラインメント |
| `Image.Width = "5cm"` | CSS `width: 5cm` | 画像サイズ |
| `Image.Height = "3cm"` | CSS `height: 3cm` | 画像サイズ |

### メソッドマッピング

| MigraDocメソッド | IronPDF/HTML相当 | 備考 |
|----------------|-------------------------|-------|
| `document.AddSection()` | `<div>` または `<section>` | 構造コンテナ |
| `section.AddParagraph("text")` | `<p>text</p>` | 段落要素 |
| `section.AddParagraph().AddText("text")` | `<p>text</p>` | 同じ結果 |
| `section.AddParagraph().AddFormattedText("text", TextFormat.Bold)` | `<p><strong>text</strong></p>` | フォーマットされたテキスト |
| `section.AddTable()` | `<table>` | テーブル要素 |
| `table.AddColumn("5cm")` | `<th style="width:5cm">` または CSS | 列幅 |
| `table.AddRow()` | `<tr>` | テーブル行 |
| `row.Cells[0].AddParagraph("text")` | `<td>text</td>` | セル内容 |
| `section.AddImage(path)` | `<img src="path">` | 画像要素 |
| `section.AddPageBreak()` | `<div style="page-break-after:always">` | ページ区切り |
| `paragraph.AddPageField()` | `{page}` ヘッダー/フッター内 | 現在のページ |
| `paragraph.AddNumPagesField()` | `{total-pages}` ヘッダー/フッター内 | 合計ページ数 |
| `section.Headers.Primary.AddParagraph()` | `RenderingOptions.HtmlHeader` | ヘッダー定義 |
| `section.Footers.Primary.AddParagraph()` | `RenderingOptions.HtmlFooter` | フッター定義 |
| `renderer.RenderDocument()` | `renderer.RenderHtmlAsPdf(html)` | PDFレンダリング |
| `pdfDocument.Save(path)` | `pdf.SaveAs(path)` | ファイルに保存 |

---

## コード移行例

### 例1：スタイリング付きのシンプルなドキュメント

**MigraDoc前：**
```csharp
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

Document document = new Document();
Section section = document.AddSection();

Paragraph title = section.AddParagraph("Welcome");
title.Format.Font.Size = 24;
title.Format.Font.Bold = true;
title.Format.Font.Color = Colors.DarkBlue;
title.Format.Alignment = ParagraphAlignment.Center;
title.Format.SpaceAfter = 20;

Paragraph body = section.AddParagraph();
body.AddText("This is regular text. ");
body.AddFormattedText("This is bold.", TextFormat.Bold);
body.AddText(" ");
body.AddFormattedText("This is italic.", TextFormat.Italic);

PdfDocumentRenderer renderer = new PdfDocumentRenderer();
renderer.Document = document;
renderer.RenderDocument();
renderer.PdfDocument.Save("output.pdf");
```

**IronPDF後：**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

string html = @"
    <html>
    <head>
        <style>
            h1 {
                font-size: 24pt;
                font-weight: bold;
                color: darkblue;
                text-align: center;
                margin-bottom: 20pt;
            }
        </style>
    </head>
    <body>
        <h1>Welcome</h1>
        <p>
            This is regular text.
            <strong>This is bold.</strong>
            <em>This is italic.</em>
        </p>
    </body>
    </html>";

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("output.pdf");
```

### 例2：スタイリング付きの