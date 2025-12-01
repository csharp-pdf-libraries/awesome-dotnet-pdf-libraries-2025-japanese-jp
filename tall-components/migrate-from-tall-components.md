---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [tall-components/migrate-from-tall-components.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/tall-components/migrate-from-tall-components.md)
🇯🇵 **日本語:** [tall-components/migrate-from-tall-components.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/tall-components/migrate-from-tall-components.md)

---

# 移行ガイド：Tall Components (TallPDF, PDFKit) → IronPDF

## Tall Componentsから移行する理由は？

Tall Components (TallPDF, PDFKit) は、Apryseによる買収後にサービスが終了し、新しいライセンスが利用できなくなり、ユーザーはiText SDKにリダイレクトされています。この製品は、XMLベースのドキュメント作成のみをサポートしており、現代のWebベースのPDF生成には不適切です。また、空白ページ、グラフィックの消失、テキストの欠落、フォントレンダリングの不正確さなど、広範囲にわたる文書化されたレンダリングバグがあり、本番アプリケーションでの使用には信頼性がありません。

## NuGetパッケージの変更

```bash
# Tall Componentsパッケージを削除
dotnet remove package TallComponents.PDF.Kit
dotnet remove package TallComponents.PDF.Layout
dotnet remove package TallComponents.PDF.Layout.Drawing

# IronPDFをインストール
dotnet add package IronPdf
```

## 名前空間のマッピング

| Tall Components | IronPDF |
|----------------|---------|
| `TallComponents.PDF.Kit` | `IronPdf` |
| `TallComponents.PDF.Layout` | `IronPdf` |
| `TallComponents.PDF.Layout.Drawing` | `IronPdf.Drawing` |
| `TallComponents.PDF.Fonts` | `IronPdf.Fonts` |
| `TallComponents.PDF.Forms` | `IronPdf.Forms` |

## APIマッピング

| Tall Components API | IronPDF API | 備考 |
|--------------------|-------------|-------|
| `Document` | `PdfDocument` | 主要なPDFドキュメントクラス |
| `Document.Pages.Add()` | `ChromePdfRenderer.RenderHtmlAsPdf()` | XMLではなくHTMLからPDFを作成 |
| `Document.Write()` | `PdfDocument.SaveAs()` | ファイルにPDFを保存 |
| `Document.Save(Stream)` | `PdfDocument.Stream` または `PdfDocument.BinaryData` | ストリーム/バイトとしてPDFを取得 |
| `XMLDocument.Generate()` | `ChromePdfRenderer.RenderHtmlAsPdf()` | レイアウトにHTMLがXMLを置き換え |
| `Page.Canvas` | 直接HTML/CSSレンダリング | 手動のキャンバスは不要 |
| `Font.FromFile()` | `IronPdf.Fonts.FontTypes` | フォント処理 |
| `TextShape` | HTML/CSSテキスト要素 | 標準のHTMLマークアップを使用 |
| `ImageShape` | `<img>` タグ in HTML | HTML経由の画像 |
| `PdfKit.Merger.Merge()` | `PdfDocument.Merge()` | PDFの結合 |
| `Page.Transformations` | CSS変換 | 変換にCSSを使用 |
| `Document.Security` | `PdfDocument.SecuritySettings` | PDFの暗号化/権限 |

## コード例

### 例 1: シンプルなPDFドキュメントの作成

**移行前 (Tall Components):**
```csharp
using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Paragraphs;

// XMLベースのレイアウトでドキュメントを作成
Document document = new Document();
Section section = document.Sections.Add();

TextParagraph paragraph = new TextParagraph();
paragraph.Text = "Hello World";
section.Paragraphs.Add(paragraph);

using (FileStream fs = new FileStream("output.pdf", FileMode.Create))
{
    document.Write(fs);
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

// HTMLからPDFを作成
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello World</h1>");
pdf.SaveAs("output.pdf");
```

### 例 2: 画像とフォーマットされたコンテンツの追加

**移行前 (Tall Components):**
```csharp
using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Paragraphs;
using TallComponents.PDF.Layout.Shapes;

Document document = new Document();
Section section = document.Sections.Add();

// 手動でフォーマットを設定してテキストを追加
TextParagraph title = new TextParagraph();
title.Text = "Report Title";
title.Font = new Font("Arial", 24);
section.Paragraphs.Add(title);

// 手動で位置を設定して画像を追加
ImageParagraph imagePara = new ImageParagraph();
imagePara.Image = new FileImage("logo.png");
section.Paragraphs.Add(imagePara);

using (FileStream fs = new FileStream("report.pdf", FileMode.Create))
{
    document.Write(fs);
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

// HTMLとCSSを使用してPDFを作成
string html = @"
    <style>
        h1 { font-family: Arial; font-size: 24px; }
    </style>
    <h1>Report Title</h1>
    <img src='logo.png' alt='Logo' />
";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("report.pdf");
```

### 例 3: 複数のPDFの統合

**移行前 (Tall Components):**
```csharp
using TallComponents.PDF.Kit;

// PdfKitを使用してPDFを統合
Document doc1 = new Document("file1.pdf");
Document doc2 = new Document("file2.pdf");

PdfKit.Merger merger = new PdfKit.Merger();
merger.Append(doc1);
merger.Append(doc2);

Document merged = merger.Merge();
using (FileStream fs = new FileStream("merged.pdf", FileMode.Create))
{
    merged.Write(fs);
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

// IronPDFでPDFを統合
var pdf1 = PdfDocument.FromFile("file1.pdf");
var pdf2 = PdfDocument.FromFile("file2.pdf");

var merged = PdfDocument.Merge(pdf1, pdf2);
merged.SaveAs("merged.pdf");
```

## よくある問題点

### 1. XMLレイアウト vs HTML/CSS
Tall ComponentsはXMLベースのレイアウト定義を使用していますが、IronPDFは標準のHTML/CSSを使用します。XMLレイアウトテンプレートをHTMLの同等物に変換してください。

**移行のヒント:** 既存のXMLデータを使用しますが、Razor、文字列補間、またはテンプレートエンジンを使用してHTMLテンプレートでレンダリングします。

### 2. キャンバスベースの描画
Tall Componentsは手動のキャンバス描画操作を必要とします。IronPDFはHTML/CSSを通じてレイアウトを自動的に処理します。

**移行のヒント:** `Canvas`、`TextShape`、および`ImageShape`の呼び出しをHTMLマークアップとCSSスタイリングに置き換えます。

### 3. フォント処理
Tall Componentsはカスタムフォントに`Font.FromFile()`を使用します。IronPDFは自動的にシステムフォントまたは埋め込みWebフォントを使用します。

**移行のヒント:** カスタムフォントにはCSSの`@font-face`を使用するか、標準のWebセーフフォントに依存します。

### 4. ストリーム処理
Tall Componentsは`Document.Write(Stream)`を使用します。IronPDFは複数の出力オプションを提供します。

**移行のヒント:** ファイルには`PdfDocument.SaveAs()`を、MemoryStreamには`PdfDocument.Stream`を、バイト配列には`PdfDocument.BinaryData`を使用します。

### 5. ページサイズと向き
Tall ComponentsはDocument/Sectionオブジェクトでページプロパティを設定します。IronPDFはCSSまたはレンダリングオプションを使用します。

**移行のヒント:** `ChromePdfRenderer.RenderingOptions.PaperSize`および`PaperOrientation`プロパティを使用するか、CSSの`@page`ルールを介して設定します。

### 6. 手動ページ管理なし
Tall Componentsは明示的なページとセクションの作成を要求します。IronPDFは自動的にページネーションを処理します。

**移行のヒント:** HTMLコンテンツを自然に流れさせます。必要に応じて手動のページブレークにはCSSの`page-break-before/after`を使用します。

### 7. セキュリティと暗号化
両方のライブラリはPDFセキュリティをサポートしていますが、APIが異なります。

**移行のヒント:** Tall Componentsの`Document.Security`プロパティの代わりに、IronPDFの`PdfDocument.SecuritySettings.SetOwnerPassword()`および`SetUserPassword()`を使用します。

## 追加リソース

- **IronPDFドキュメント:** https://ironpdf.com/docs/
- **IronPDFチュートリアル:** https://ironpdf.com/tutorials/
- **HTMLからPDFへのガイド:** https://ironpdf.com/docs/questions/html-to-pdf/
- **APIリファレンス:** https://ironpdf.com/object-reference/api/