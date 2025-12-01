---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [vectsharp/migrate-from-vectsharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/vectsharp/migrate-from-vectsharp.md)
🇯🇵 **日本語:** [vectsharp/migrate-from-vectsharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/vectsharp/migrate-from-vectsharp.md)

---

# 移行ガイド: VectSharp → IronPDF

## VectSharpからIronPDFへ移行する理由

VectSharpはベクターグラフィックスと科学的視覚化のために設計されており、ドキュメント生成やHTMLベースのPDFワークフローには適していません。IronPDFはHTML、URL、ドキュメントからPDFを作成することに優れており、最新のWeb標準、CSS、JavaScriptを完全にサポートしています。ビジネスドキュメント、レポートの生成やWebコンテンツをPDFに変換する必要がある場合、IronPDFはより適切で機能豊富なソリューションを提供します。

## NuGetパッケージの変更

```bash
# VectSharpを削除
dotnet remove package VectSharp
dotnet remove package VectSharp.PDF

# IronPDFを追加
dotnet add package IronPdf
```

## 名前空間のマッピング

| VectSharp | IronPDF |
|-----------|---------|
| `VectSharp` | `IronPdf` |
| `VectSharp.PDF` | `IronPdf` |
| `VectSharp.Graphics` | `IronPdf` (HTML/CSSベース) |
| `VectSharp.Canvas` | 該当なし (HTML canvasを使用) |

## APIマッピング

| VectSharpの概念 | IronPDFの相当する機能 |
|-------------------|-------------------|
| `Document` | `ChromePdfRenderer` |
| `Page` | HTMLページ / ページ区切り |
| `Graphics.FillRectangle()` | CSSスタイリングを施したHTMLの`<div>` |
| `Graphics.StrokePath()` | HTML内のSVGまたはHTML5 Canvas |
| `SaveAsPDF()` | `RenderHtmlAsPdf()` / `RenderUrlAsPdf()` |
| ベクター描画コマンド | HTML/CSS/SVGマークアップ |
| `Colour` | CSSの色値 |
| 手動ページサイズ設定 | `PdfPrintOptions`のページ設定 |

## コード例

### 例1: シンプルなPDFの作成

**VectSharpでの方法:**
```csharp
using VectSharp;
using VectSharp.PDF;

Document doc = new Document();
Page page = new Page(595, 842); // A4サイズ
Graphics graphics = page.Graphics;

graphics.FillRectangle(50, 50, 200, 100, Colour.FromRgb(0, 0, 255));
graphics.FillText(60, 70, "Hello from VectSharp", 
    new Font(new FontFamily("Arial"), 20), Colour.FromRgb(255, 255, 255));

doc.Pages.Add(page);
doc.SaveAsPDF("output.pdf");
```

**IronPDFでの方法:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

string html = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        .box {
            width: 200px;
            height: 100px;
            background-color: blue;
            padding: 20px;
            margin: 50px;
        }
        h1 {
            color: white;
            font-family: Arial;
            font-size: 20px;
        }
    </style>
</head>
<body>
    <div class='box'>
        <h1>Hello from IronPDF</h1>
    </div>
</body>
</html>";

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("output.pdf");
```

### 例2: マルチページドキュメント

**VectSharpでの方法:**
```csharp
using VectSharp;
using VectSharp.PDF;

Document doc = new Document();

for (int i = 1; i <= 3; i++)
{
    Page page = new Page(595, 842);
    Graphics graphics = page.Graphics;
    
    graphics.FillText(50, 50, $"Page {i}", 
        new Font(new FontFamily("Arial"), 24), Colours.Black);
    
    doc.Pages.Add(page);
}

doc.SaveAsPDF("multipage.pdf");
```

**IronPDFでの方法:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

string html = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial; font-size: 24px; }
        .page-break { page-break-after: always; }
    </style>
</head>
<body>
    <div class='page-break'><h1>Page 1</h1></div>
    <div class='page-break'><h1>Page 2</h1></div>
    <div><h1>Page 3</h1></div>
</body>
</html>";

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("multipage.pdf");
```

### 例3: カスタムページサイズと描画

**VectSharpでの方法:**
```csharp
using VectSharp;
using VectSharp.PDF;

Document doc = new Document();
Page page = new Page(800, 600); // カスタムサイズ
Graphics graphics = page.Graphics;

// 円を描画
GraphicsPath path = new GraphicsPath();
path.Arc(400, 300, 100, 0, 2 * Math.PI);
graphics.FillPath(path, Colour.FromRgb(255, 0, 0));

// テキストを追加
graphics.FillText(350, 290, "Circle", 
    new Font(new FontFamily("Arial"), 16), Colours.White);

doc.Pages.Add(page);
doc.SaveAsPDF("custom.pdf");
```

**IronPDFでの方法:**
```csharp
using IronPdf;
using IronPdf.Rendering;

var renderer = new ChromePdfRenderer();

// カスタムページサイズを設定
renderer.RenderingOptions.PaperSize = PdfPaperSize.Custom;
renderer.RenderingOptions.CustomPaperWidth = 800;
renderer.RenderingOptions.CustomPaperHeight = 600;

string html = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { margin: 0; }
        svg { display: block; }
        text { fill: white; font-family: Arial; font-size: 16px; }
    </style>
</head>
<body>
    <svg width='800' height='600'>
        <circle cx='400' cy='300' r='100' fill='red' />
        <text x='360' y='305'>Circle</text>
    </svg>
</body>
</html>";

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("custom.pdf");
```

## よくある問題点

1. **パラダイムシフト**: VectSharpは命令型の描画コマンドを使用していますが、IronPDFは宣言型のHTML/CSSを使用します。描画ロジックをHTMLマークアップとスタイリングに変換する必要があります。

2. **直接描画APIがない**: IronPDFには`FillRectangle()`や`DrawLine()`のようなメソッドがありません。ベクターグラフィックスにはHTML内の要素(`<div>`、`<span>`)やSVGを使用してください。

3. **ページサイズ**: VectSharpは直接ポイントを使用しますが、IronPDFは`PaperSize`列挙型や`RenderingOptions`を通じたカスタム寸法を使用します。レンダリング前にページサイズを設定してください。

4. **フォント処理**: VectSharpではFontオブジェクトが必要ですが、IronPDFではCSSの`font-family`を使用します。システムにフォントがインストールされていることを確認するか、Webフォントを使用してください。

5. **座標系**: VectSharpはデフォルトで左下原点を使用しますが、HTML/CSSは左上を使用します。位置調整を適切に行ってください。

6. **パフォーマンス**: IronPDFはブラウザエンジンを通じてレンダリングするため、単純なグラフィックスの直接ベクター描画に比べてVectSharpより遅くなる可能性がありますが、はるかに柔軟性があります。

7. **ライセンス**: IronPDFは商用利用にライセンスが必要ですが、VectSharpはオープンソースです。ライセンス要件についてはhttps://ironpdf.com/licensing/を確認してください。

## 追加リソース

- **IronPDFドキュメント**: https://ironpdf.com/docs/
- **チュートリアル & 例**: https://ironpdf.com/tutorials/
- **HTMLからPDFへのガイド**: https://ironpdf.com/docs/questions/html-to-pdf/
- **APIリファレンス**: https://ironpdf.com/object-reference/api/