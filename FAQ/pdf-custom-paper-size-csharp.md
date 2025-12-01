---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [FAQ/pdf-custom-paper-size-csharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/pdf-custom-paper-size-csharp.md)
🇯🇵 **日本語:** [FAQ/pdf-custom-paper-size-csharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/pdf-custom-paper-size-csharp.md)

---

# C#でIronPDFを使用してカスタムサイズのPDFを生成する方法は？

C#でA4やレターサイズ以外のPDFを作成するのに苦労したことがある方は、あなただけではありません。配送ラベル、領収書、名刺、さらには巨大なポスターまで、正確なPDF用紙サイズを設定できることは必須です。IronPDFはこのプロセスを簡単にし、用紙の寸法、余白、向きなどを指定できるため、PDFが実際のニーズに常に一致します。

以下では、.NETでIronPDFを使用してカスタムサイズのPDFを生成する際の開発者からのよくある質問に答え、コードサンプルとトラブルシューティングのヒントを提供します。

---

## C#でカスタムPDF用紙サイズが必要な理由は？

多くの実世界のPDFシナリオでは、標準のA4やレターサイズを超えるサイズが必要です。例えば：

- **配送ラベル**（4インチx6インチなど）
- **サーマル領収書**（幅80mm、長さ可変）
- **名刺**、チケット、またはリストバンド
- **大型ポスター**やバナー

プリンター、郵便サービス、さらにはラベルメーカーも、非常に特定の用紙寸法を要求することがよくあります。IronPDFを使用すると、通常の標準に縛られることなく、あらゆる種類の物理フォーマットのPDFをカスタマイズできます。異なるマークアップ形式をPDFに変換する方法の詳細については、[C#でXMLをPDFに変換する方法は？](xml-to-pdf-csharp.md)または[.NET MAUIでXAMLからPDFを生成する方法は？](xaml-to-pdf-maui-csharp.md)を参照してください。

---

## C#でカスタムサイズのPDFを迅速に作成する方法は？

IronPDFを使用してPDFを生成する際に、任意の用紙サイズを指定できます。例えば、100mm x 150mmの配送ラベルが必要な場合、実用的な例を以下に示します：

```csharp
using IronPdf;
// Install-Package IronPdf

var renderer = new ChromePdfRenderer();

// 用紙サイズを100mm x 150mmに設定
renderer.RenderingOptions.SetCustomPaperSizeInMillimeters(100, 150);

// 配送ラベルのHTML例
var html = @"
    <style>
        body { font-family: Arial, sans-serif; }
        .address { font-size: 14px; margin-bottom: 18px; }
        .barcode { font-size: 25px; letter-spacing: 2px; }
    </style>
    <div class='address'>
        <strong>Jane Smith</strong><br>
        456 Oak Street<br>
        Hometown, CA 90210
    </div>
    <div class='barcode'>1Z999AA10123456784</div>
";
var pdfDoc = renderer.RenderHtmlAsPdf(html);
pdfDoc.SaveAs("shipping-label.pdf");
```

この方法では、出力サイズを完全に制御でき、PDFがプリンターやラベルストックに正確に合うようになります。

---

## IronPDFはどの標準用紙サイズをサポートしていますか？

IronPDFには、寸法を調べる必要がない幅広い内蔵用紙サイズがあります。`PaperSize`プロパティを設定するだけです：

```csharp
using IronPdf;
// Install-Package IronPdf

var renderer = new ChromePdfRenderer();

renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;
renderer.RenderingOptions.PaperSize = PdfPaperSize.A5;

var pdf = renderer.RenderHtmlAsPdf("<h1>Standard size example</h1>");
pdf.SaveAs("standard.pdf");
```

### なぜプリセットサイズを使用するのですか？

- 寸法の間違いを防ぐ
- 一般的な国際基準または北米基準に合わせる
- プリンターやビューアとの互換性問題を避ける

一般的なサイズの便利な参照表はこちらです：

| 名称      | mm (幅x高さ)        | インチ (幅x高さ)      | 一般的な用途         |
|-----------|-----------------|---------------|--------------------|
| A4        | 210 x 297       | 8.27 x 11.69  | 世界のほとんどの文書    |
| レター    | 216 x 279       | 8.5 x 11      | 米国のフォーム           |
| リーガル     | 216 x 356       | 8.5 x 14      | 法的文書         |
| A3        | 297 x 420       | 11.69 x 16.54 | ポスター、図表  |
| A5        | 148 x 210       | 5.83 x 8.27   | チラシ、小冊子   |

---

## C#でPDFのカスタム用紙サイズを設定する方法は？

サーマルプリンターや非標準ラベルなど、非常に特定のサイズが必要な場合は、ミリメートル、インチ、またはポイントで寸法を指定できます。それぞれの方法は以下の通りです：

```csharp
using IronPdf;
// Install-Package IronPdf

var renderer = new ChromePdfRenderer();

// ミリメートル単位
renderer.RenderingOptions.SetCustomPaperSizeInMillimeters(80, 200);

// インチ単位
renderer.RenderingOptions.SetCustomPaperSizeInInches(3.5, 2);

// ポイント単位（1インチあたり72ポイント）
renderer.RenderingOptions.SetCustomPaperSizeInPixelsOrPoints(252, 144);
```

**ヒント：** 常にプリンターのドキュメントを確認して、サポートされているサイズと余白の要件を確認してください。一部のプリンターは非常に特定の要件を持っています。

---

## カスタムPDFサイズの実世界の使用例は？

IronPDFの柔軟性は、さまざまな印刷シナリオで役立ちます。いくつかの実用的な例を以下に示します：

### サーマル領収書PDFを生成する方法は？

サーマルプリンターはしばしば狭く長いページを必要とします。80mm幅、300mm長の領収書を設定する方法は以下の通りです：

```csharp
using IronPdf;
// Install-Package IronPdf

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.SetCustomPaperSizeInMillimeters(80, 300);

renderer.RenderingOptions.MarginTop = 3;
renderer.RenderingOptions.MarginBottom = 3;
renderer.RenderingOptions.MarginLeft = 3;
renderer.RenderingOptions.MarginRight = 3;

string receiptHtml = @"
<style>
    body { font-family: monospace; font-size: 11px; }
    .center { text-align: center; }
    hr { border-top: 1px dashed #333; }
</style>
<div class='center'>
    <h3>Pizza Place</h3>
    <p>789 Elm Road</p>
</div>
<hr>
<p>Large Pizza ...... $15.99</p>
<p>Soda ............ $1.99</p>
<hr>
<p><strong>Total: $17.98</strong></p>
<div class='center'><p>Thanks for your order!</p></div>
";

var pdf = renderer.RenderHtmlAsPdf(receiptHtml);
pdf.SaveAs("receipt.pdf");
```

領収書の長さが変わる場合は、ページの高さを動的に設定するか、複数ページを生成することを検討してください。

### PDFとして名刺を印刷する方法は？

標準的な名刺は3.5インチx2インチです。正確なサイズのカードを生成する方法は以下の通りです：

```csharp
using IronPdf;
// Install-Package IronPdf

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.SetCustomPaperSizeInInches(3.5, 2);

renderer.RenderingOptions.MarginTop = 3;
renderer.RenderingOptions.MarginBottom = 3;
renderer.RenderingOptions.MarginLeft = 3;
renderer.RenderingOptions.MarginRight = 3;

string cardHtml = @"
<style>
    body { font-family: 'Segoe UI', Arial; margin: 0; }
    .container {
        display: flex; flex-direction: column; align-items: center; justify-content: center;
        height: 100%; padding: 6px;
    }
    h2 { margin: 0; font-size: 16px; }
    p { margin: 2px 0; font-size: 10px; }
</style>
<div class='container'>
    <h2>Jane Developer</h2>
    <p>Full Stack Engineer</p>
    <p>jane@yourcompany.com</p>
    <p>+1 555 321 7654</p>
</div>
";

var pdf = renderer.RenderHtmlAsPdf(cardHtml);
pdf.SaveAs("business-card.pdf");
```

1ページに複数のカードを印刷する場合は、標準ページサイズを使用し、CSSグリッドを使用して1枚の用紙に複数のカードをレイアウトします。

### 大型ポスターPDFを作成する方法は？

たとえば24インチx36インチのポスターが必要な場合、それは同じくらい簡単です：

```csharp
using IronPdf;
// Install-Package IronPdf

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.SetCustomPaperSizeInInches(24, 36);

string posterHtml = @"
<style>
    body { 
        display: flex; align-items: center; justify-content: center;
        height: 100%; background: #fff;
    }
    h1 { font-size: 170px; color: #d32f2f; font-family: Impact, sans-serif; margin: 0; }
</style>
<h1>SALE!</h1>
";

var pdf = renderer.RenderHtmlAsPdf(posterHtml);
pdf.SaveAs("poster.pdf");
```

本当に大きなサイズを必要とする場合は、印刷所に確認し、複数のビューアでPDFをテストしてください。

### 1枚の用紙に複数のラベルを印刷する方法は？

レター用紙に10枚のラベルを印刷する場合は、CSSを使用してレイアウトを設定し、ページサイズをレターに設定します：

```csharp
using IronPdf;
// Install-Package IronPdf

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;
renderer.RenderingOptions.MarginTop = 10;
renderer.RenderingOptions.MarginBottom = 10;
renderer.RenderingOptions.MarginLeft = 10;
renderer.RenderingOptions.MarginRight = 10;

string labelSheetHtml = @"
<style>
    .labels { display: flex; flex-wrap: wrap; width: 100%; }
    .label {
        width: 180px; height: 90px;
        border: 1px solid #333; margin: 5px; padding: 6px;
        font-family: Arial; font-size: 12px;
        display: flex; align-items: center; justify-content: center;
    }
</style>
<div class='labels'>
    <div class='label'>Label 1</div>
    <div class='label'>Label 2</div>
    <div class='label'>Label 3</div>
    <div class='label'>Label 4</div>
    <div class='label'>Label 5</div>
    <div class='label'>Label 6</div>
    <div class='label'>Label 7</div>
    <div class='label'>Label 8</div>
    <div class='label'>Label 9</div>
    <div class='label'>Label 10</div>
</div>
";

var pdf = renderer.RenderHtmlAsPdf(labelSheetHtml);
pdf.SaveAs("label-sheet.pdf");
```

完璧なアライメントのために、CSSの寸法を実際のラベルストックに合わせてください。ウェブフォントやアイコンを使用した高度なラベルシナリオについては、[PDFでウェブフォントとアイコンを使用する方法は？](web-fonts-icons-pdf-csharp.md)が役立つかもしれません。

---

## 同じソースから複数の用紙サイズのPDFを生成する方法は？

A4、レター、A5など、同じコンテンツをさまざまな形式で出力する必要がある場合は、シンプルなループで自動化します：

```csharp
using IronPdf;
// Install-Package IronPdf
using System.Collections.Generic;

string html = "<h1>Multi-size PDF</h1><p>This prints at different dimensions.</p>";

var sizes = new Dictionary<string, (double width, double height)>
{
    { "a4", (210, 297) },
    { "letter", (216, 279) },
    { "a5", (148, 210) }
};

foreach (var (name, (width, height)) in sizes)
{
    var renderer = new ChromePdfRenderer();
    renderer.RenderingOptions.SetCustomPaperSizeInMillimeters(width, height);

    var pdf = renderer.RenderHtmlAsPdf(html);
    pdf.SaveAs($"doc-{name}.pdf");
}
```

これは、国際的な顧客にサービスを提供する印刷ポータルやアプリに非常に便利です。

---

## PDFの向き（縦向き対横向き）を制御する方法は？

デフォルトでは、幅が高さよりも大きい場合、IronPDFはそれを横向きとして扱います。しかし、向きを明示的に設定することもできます：

```csharp
using IronPdf;
// Install-Package IronPdf

var renderer = new ChromePdfRenderer();

// A4横向きにプリセットを設定
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;

// または、カスタム横向きサイズを手動で設定
renderer.RenderingOptions.SetCustomPaperSizeInMillimeters(297, 210);
```

カスタムサイズを使用する場合、幅と高さの順序が向きを決定します。

---

## ページサイズに対してPDFコンテンツをレスポンシブにする方法は？

時には、HTMLコンテンツがページに合わない場合や、スケールする必要があります。IronPDFはこれを支援する機能を提供します：

### コンテンツを用紙に合わせてスケールする方法は？

`FitToPaperMode`を設定してコンテンツがスケールするようにします：

```csharp
using Iron