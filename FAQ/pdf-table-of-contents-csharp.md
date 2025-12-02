# C# PDFでクリック可能な目次を作成する方法は？

長いPDFを見失ったことがあれば、「セクション7.4.2」を無限にスクロールして探す苦痛を知っているでしょう。よく作られたクリック可能な目次（TOC）はPDFを変身させ、特に報告書、マニュアル、または電子書籍にとって、それらを簡単にナビゲートできるようにします。C#では、IronPDFを使ってカスタムスタイリングとブックマーク付きのインタラクティブなTOCを驚くほど簡単に生成できます。このFAQは、始め方から高度なテクニックまで全てをカバーしているので、読者にとって真に機能するPDFを構築できます。

---

## なぜC# PDFに目次を追加すべきで、IronPDFはどのように役立つのか？

目次を追加することは見た目だけでなく、使いやすさについてです。PDFが技術マニュアルであろうとビジネス提案であろうと、目次は読者にロードマップと各重要セクションへのクリック可能なリンクを提供します。IronPDFは人気があります。なぜなら、HTMLの見出し（`<h1>`–`<h6>`）を自動的にスキャンし、階層リンクとオプションのスタイリングを備えたTOCを生成するからです。さらに、サイドバーのナビゲーション用のPDFブックマークをサポートし、ユーザーが章間を努力なくジャンプできるようにします。

実際に動作を見たいですか？以下のサンプルコードを試してみてください—ナビゲーション可能なPDFまで10行未満です！

---

## IronPDFを使用してC#で自動的に目次を生成する方法は？

IronPDFは、ほとんど追加コードなしでHTMLからクリック可能なTOCを生成できます。これが簡単な方法です：

```csharp
using IronPdf; // NuGet: Install-Package IronPdf

var pdfCreator = new ChromePdfRenderer();
pdfCreator.RenderingOptions.TableOfContents = TableOfContentsTypes.WithLinks;

// 見出しを含むサンプルHTML
var html = @"
<h1>概要</h1>
<p>ここから始めてください...</p>
<h2>背景</h2>
<p>背景の詳細...</p>
<h1>メインセクション</h1>
<h2>詳細</h2>
";

var pdfDoc = pdfCreator.RenderHtmlAsPdf(html);
pdfDoc.SaveAs("toc-sample.pdf");
```

生成されたPDFを開くと、各エントリがセクションにリンクしている目次が最初に見つかります。IronPDFは見出しを自動的に検出し、階層を構築します。

XMLまたはXAMLをPDFに変換する方法については、[C#でXMLをPDFに変換する方法は？](xml-to-pdf-csharp.md)と[.NET MAUIでXAMLをPDFに変換する方法は？](xaml-to-pdf-maui-csharp.md)を参照してください。

---

## IronPDFの自動TOC検出はどのように機能するのか？

IronPDFはHTMLを解析し、見出し（`<h1>`から`<h6>`まで）を探します。それから階層（例えば、`h1`をトップレベル、`h2`をサブヘッダーとして）を確立し、通常は最初に、各見出しへのクリック可能なリンクを含むTOCページを生成します。これらのリンクは、どの主流のPDFリーダーでも機能します。HTMLでセマンティックな見出しを使用する限り、IronPDFが残りを処理します。

---

## PDF内で目次が表示される場所をどのように制御できるか？

デフォルトでは、IronPDFは文書の最初に目次を配置します。しかし、他の場所に移動したい場合は、HTMLにプレースホルダーを追加するだけです：

```csharp
using IronPdf; // NuGet: Install-Package IronPdf

var pdfRenderer = new ChromePdfRenderer();
pdfRenderer.RenderingOptions.TableOfContents = TableOfContentsTypes.WithLinks;

var htmlContent = @"
<html>
  <body>
    <h1>前文</h1>
    <p>ようこそ！</p>
    <div id='ironpdf-toc'></div>
    <h1>第一章</h1>
    <p>内容...</p>
  </body>
</html>
";

var outputPdf = pdfRenderer.RenderHtmlAsPdf(htmlContent);
outputPdf.SaveAs("custom-toc-location.pdf");
```
目的の場所に`<div id='ironpdf-toc'></div>`を挿入するだけで、目次がそこにレンダリングされます！

---

## 目次をカスタムCSSでスタイルする方法は？

IronPDFはTOC要素に特定のCSSクラスを追加し、必要に応じてスタイルを設定できるようにします。カスタムルックを作成する方法は次のとおりです：

```csharp
using IronPdf; // NuGet: Install-Package IronPdf

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.TableOfContents = TableOfContentsTypes.WithLinks;

var styledHtml = @"
<html>
<head>
  <style>
    .ironpdf-toc { background: #eaeaea; padding: 1em; border-radius: 6px; }
    .ironpdf-toc-title { font-size: 24px; font-weight: bold; color: #114488; }
    .ironpdf-toc-item-h1 { margin-left: 0; }
    .ironpdf-toc-item-h2 { margin-left: 16px; color: #333; }
    .ironpdf-toc-item-h3 { margin-left: 32px; color: #888; }
    .ironpdf-toc-item a { color: #0056b3; text-decoration: none; }
    .ironpdf-toc-item a:hover { text-decoration: underline; }
  </style>
</head>
<body>
  <div id='ironpdf-toc'></div>
  <h1>ここから始める</h1>
  <h2>インストール</h2>
  <h2>設定</h2>
  <h1>高度な設定</h1>
  <h2>オプション</h2>
</body>
</html>
";

var pdf = renderer.RenderHtmlAsPdf(styledHtml);
pdf.SaveAs("custom-styled-toc.pdf");
```
`.ironpdf-toc`、`.ironpdf-toc-title`、`.ironpdf-toc-item-hX`のようなクラスをターゲットにして、外観を細かく制御します。

カスタムフォントとアイコンについては、[PDF出力でウェブフォントとアイコンを使用する方法は？](web-fonts-icons-pdf-csharp.md)をチェックしてください。

---

## 目次に含まれる見出しを制限することはできるか？

はい！文書が非常に長い場合、目次に特定の見出しレベルのみを表示したいかもしれません。これは少しのCSSで実現できます：

```csharp
using IronPdf;

var pdfRenderer = new ChromePdfRenderer();
pdfRenderer.RenderingOptions.TableOfContents = TableOfContentsTypes.WithLinks;

var html = @"
<html>
<head>
  <style>
    .ironpdf-toc-item-h3,
    .ironpdf-toc-item-h4,
    .ironpdf-toc-item-h5,
    .ironpdf-toc-item-h6 { display: none; }
  </style>
</head>
<body>
  <div id='ironpdf-toc'></div>
  <h1>パート1</h1>
  <h2>サブパート1.1</h2>
  <h3>詳細1.1.1</h3> <!-- 目次に表示されない -->
  <h1>パート2</h1>
</body>
</html>
";

var pdfOutput = pdfRenderer.RenderHtmlAsPdf(html);
pdfOutput.SaveAs("filtered-toc.pdf");
```
このCSSを使用すると、`h1`と`h2`のみが目次に表示されます。

---

## 目次にページ番号を追加する方法は？

クラシックな目次は点線でページ番号を表示します。IronPDFはレンダリング時にこれらの番号を自動的に更新することはできませんが、CSSを使用して効果を模倣し、一貫性のためにフッターに実際のページ番号を追加することができます：

```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.TableOfContents = TableOfContentsTypes.WithLinks;
renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = "<div style='text-align:center; font-size:10px;'>Page {page} of {total-pages}</div>"
};

var html = @"
<html>
<head>
  <style>
    .ironpdf-toc-item {
      display: flex;
      justify-content: space-between;
      padding-right: 1em;
    }
    .ironpdf-toc-item-text { flex: 1; }
    .ironpdf-toc-item-page::before {
      content: '.........................';
      color: #ccc;
      margin-right: 8px;
    }
  </style>
</head>
<body>
  <div id='ironpdf-toc'></div>
  <h1>第一章</h1>
  <h2>導入</h2>
</body>
</html>
";

var finalPdf = renderer.RenderHtmlAsPdf(html);
finalPdf.SaveAs("toc-with-pages.pdf");
```
リンクはクリック可能のままで、ユーザーはフッターのページ番号を参照できます。[ページ番号を追加する](https://ironpdf.com/blog/compare-to-other-components/questpdf-add-page-number-to-pdf/)方法についての公式IronPDFドキュメントをご覧ください。

---

## サイドバーナビゲーション用のブックマークを追加または編集する方法は？

ブックマークは、特に技術文書や電子書籍で、PDFにサイドバーナビゲーションを提供する強力な方法です。

### プログラムでブックマークを挿入する方法は？

PDF作成中または作成後に特定のページへのブックマークを追加できます：

```csharp
using IronPdf;

var pdfDoc = PdfDocument.FromFile("manual.pdf");

pdfDoc.Bookmarks.AddBookMarkAtEnd("概要", 0);
pdfDoc.Bookmarks.AddBookMarkAtEnd("詳細", 5);
pdfDoc.Bookmarks.AddBookMarkAtEnd("結論", 10);

pdfDoc.SaveAs("with-bookmarks.pdf");
```
ブックマークはPDFリーダーのサイドバーに表示され、ユーザーが重要なセクションにジャンプできるようにします。

### 章と小節のためにネストされたブックマークを作成できるか？

もちろんです。ブックマークは階層的なナビゲーションのためにネストできます：

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("guide.pdf");

var ch1 = pdf.Bookmarks.AddBookMarkAtEnd("第1章", 0);
ch1.Children.AddBookMarkAtEnd("1.1 スタートガイド", 2);
ch1.Children.AddBookMarkAtEnd("1.2 セットアップ", 4);

var ch2 = pdf.Bookmarks.AddBookMarkAtEnd("第2章", 10);
ch2.Children.AddBookMarkAtEnd("2.1 ディープダイブ", 12);

pdf.SaveAs("nested-bookmarks.pdf");
```
これは複雑なマニュアルや参考書に理想的です。

### 既存のブックマークを読み取るまたは更新する方法は？

次のようにしてブックマークを反復処理し、変更することができます：

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("existing-bookmarks.pdf");
var allBookmarks = pdf.Bookmarks.GetAllBookmarks();

foreach (var bm in allBookmarks)
{
    Console.WriteLine($"ブックマーク: {bm.Title}, ページ: {bm.PageIndex}");
    foreach (var child in bm.Children)
    {
        Console.WriteLine($"  サブ: {child.Title}, ページ: {child.PageIndex}");
    }
}
```
これは既存の文書のナビゲーションを監査または更新するのに便利です。

PDFを分割、マージ、または整理する方法については、[C#でPDFを分割、マージ、または整理する方法は？](organize-merge-split-pdfs-csharp.md)を参照してください。

---

## PDFをマージしてナビゲーションを維持する際に知っておくべきことは？

複数のPDFをマージする場合、見出しタグがなくなると自動生成されたTOCは機能しないため、HTMLから始める場合にのみTOCを自動生成できることに注意してください。しかし、マージされた文書にブックマークを追加できます：

```csharp
using IronPdf;

var firstPart = PdfDocument.FromFile("section1.pdf");
var secondPart = PdfDocument.FromFile("section2.pdf");
var merged = PdfDocument.Merge(firstPart, secondPart);

merged.Bookmarks.AddBookMarkAtEnd("セクション1", 0);
merged.Bookmarks.AddBookMarkAtEnd("セクション2", firstPart.PageCount);

merged.SaveAs("merged-with-nav.pdf");
```
マージ技術の詳細については、[このIronPDFチュートリアル](https://ironpdf.com/java/how-to/java-merge-pdf-tutorial/)をチェックしてください。

---

## 動的データやローカライズされたコンテンツからTOCを生成する方法は？

### データベースなどの動的ソースからTOCを構築する方法は？

必要に応じて見出しを含むHTML文字列を生成し、IronPDFがそれらを拾います：

```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.TableOfContents = TableOfContentsTypes.WithLinks;

var chapters = new[]
{
    new { Name = "導入", Body = "ようこそ..." },
    new { Name = "インストール", Body = "インストール手順..." },
};

var html = "<div id='ironpdf-toc'></div>";
foreach (var ch in chapters)
{
    html += $"<h1>{ch.Name}</h1><p>{ch.Body}</p>";
}

var doc = renderer.RenderHtmlAsPdf(html);
doc.SaveAs("dynamic-content-toc.pdf");
```
これはアプリ生成のレポートやユーザー主導のコンテンツに最適です。

### TOCと見出しをローカライズする方法は？

見