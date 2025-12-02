# VectSharpからIronPDFへの移行方法は？

## なぜVectSharpから移行するのか？

VectSharpは、図表、チャート、技術イラストを作成するために設計された科学的可視化およびベクターグラフィックスライブラリです。**ドキュメント生成には設計されていません** - PDF出力が可能な描画ライブラリです。移行する主な理由は以下の通りです：

1. **科学的焦点のみ**：ドキュメントではなく、データ可視化とプロットに設計されています
2. **HTMLサポートなし**：HTML/CSSをPDFに変換できず、手動でベクター描画が必要です
3. **座標ベースのAPI**：すべての要素を正確なX,Y座標で位置付けする必要があります
4. **CSSスタイリングなし**：Webスタイリングのサポートがなく、すべてのスタイリングがプログラムで行われます
5. **JavaScriptなし**：動的なWebコンテンツをレンダリングできません
6. **テキストレイアウトなし**：自動テキストラッピング、ページネーション、フローレイアウトがありません
7. **グラフィックスファーストのパラダイム**：レポートや請求書ではなく、図表に設計されています

### 核心問題：グラフィックスライブラリ vs ドキュメントジェネレータ

VectSharpは、図やプロットを作成する科学者向けに構築されており、ビジネスドキュメントを生成する開発者向けではありません：

```csharp
// VectSharp: すべての要素に対して手動でベクター描画
Page page = new Page(595, 842);
Graphics graphics = page.Graphics;
graphics.FillRectangle(50, 50, 200, 100, Colour.FromRgb(0, 0, 255));
graphics.FillText(60, 70, "Invoice", new Font(new FontFamily("Arial"), 20), Colours.White);
// ... すべての要素を手動で続けて描画 ...
```

IronPDFはHTML - 普遍的なドキュメントフォーマットを使用します：

```csharp
// IronPDF: ドキュメント作成のための宣言的HTML
var html = "<h1>Invoice</h1><p>Customer: Acme Corp</p>";
var pdf = renderer.RenderHtmlAsPdf(html);
```

---

## VectSharpからIronPDFへのクイックスタート

### ステップ1: NuGetパッケージの置き換え

```bash
# VectSharpパッケージを削除
dotnet remove package VectSharp
dotnet remove package VectSharp.PDF

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2: 名前空間の更新

```csharp
// 以前
using VectSharp;
using VectSharp.PDF;

// 以後
using IronPdf;
```

### ステップ3: ライセンスの初期化

```csharp
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

---

## APIマッピングリファレンス

| VectSharp | IronPDF | メモ |
|-----------|---------|-------|
| `Document` | `ChromePdfRenderer` | レンダラーを作成 |
| `Page` | 自動 | HTMLからページを作成 |
| `Graphics` | HTML/CSS | 宣言的マークアップ |
| `graphics.FillRectangle()` | `<div>`上のCSS `background-color` | HTMLボックス |
| `graphics.StrokeRectangle()` | `<div>`上のCSS `border` | 境界線 |
| `graphics.FillText()` | HTMLテキスト要素 | `<p>`, `<h1>`, `<span>` |
| `graphics.StrokePath()` | SVGまたはCSS境界線 | ベクターパス |
| `GraphicsPath` | SVG `<path>`要素 | 複雑な形状 |
| `Colour.FromRgb()` | CSSカラー値 | `rgb()`, `#hex`, 名前付き |
| `Font` / `FontFamily` | CSS `font-family` | Webフォント対応 |
| `doc.SaveAsPDF()` | `pdf.SaveAs()` | ファイルに保存 |
| 手動ページサイズ | `RenderingOptions.PaperSize` | またはCSS `@page` |

---

## コード例

### 例1: 基本的なドキュメント作成

**VectSharp:**
```csharp
using VectSharp;
using VectSharp.PDF;

Document doc = new Document();
Page page = new Page(595, 842); // A4サイズ（ポイント）
Graphics graphics = page.Graphics;

graphics.FillRectangle(50, 50, 200, 100, Colour.FromRgb(0, 0, 255));
graphics.FillText(60, 70, "Hello from VectSharp",
    new Font(new FontFamily("Arial"), 20), Colour.FromRgb(255, 255, 255));

doc.Pages.Add(page);
doc.SaveAsPDF("output.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

var html = @"
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
            font-family: Arial, sans-serif;
            font-size: 20px;
            margin: 0;
        }
    </style>
</head>
<body>
    <div class='box'>
        <h1>Hello from IronPDF</h1>
    </div>
</body>
</html>";

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("output.pdf");
```

### 例2: 複数ページのドキュメント

**VectSharp:**
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

**IronPDF:**
```csharp
using IronPdf;

var html = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; }
        .page { page-break-after: always; padding: 50px; }
        .page:last-child { page-break-after: auto; }
        h1 { font-size: 24px; }
    </style>
</head>
<body>
    <div class='page'><h1>Page 1</h1></div>
    <div class='page'><h1>Page 2</h1></div>
    <div class='page'><h1>Page 3</h1></div>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("multipage.pdf");
```

### 例3: 科学チャート（VectSharpの特長）

**VectSharp:**
```csharp
using VectSharp;
using VectSharp.PDF;

Document doc = new Document();
Page page = new Page(800, 600);
Graphics graphics = page.Graphics;

// チャート軸を描画
graphics.StrokePath(new GraphicsPath().MoveTo(50, 550).LineTo(50, 50), Colours.Black, 2);
graphics.StrokePath(new GraphicsPath().MoveTo(50, 550).LineTo(750, 550), Colours.Black, 2);

// データポイントを描画
double[] data = { 100, 250, 180, 320, 280, 450 };
double barWidth = 80;
for (int i = 0; i < data.Length; i++)
{
    double x = 80 + i * 110;
    double height = data[i];
    graphics.FillRectangle(x, 550 - height, barWidth, height, Colour.FromRgb(0, 102, 204));
}

// ラベル
for (int i = 0; i < data.Length; i++)
{
    double x = 80 + i * 110 + barWidth / 2 - 10;
    graphics.FillText(x, 565, $"Q{i + 1}",
        new Font(new FontFamily("Arial"), 12), Colours.Black);
}

graphics.FillText(350, 30, "Quarterly Sales",
    new Font(new FontFamily("Arial"), 24), Colours.Black);

doc.Pages.Add(page);
doc.SaveAsPDF("chart.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

// IronPDFは任意のJavaScriptチャートライブラリをレンダリングできます！
var html = @"
<!DOCTYPE html>
<html>
<head>
    <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
    <style>
        body { font-family: Arial, sans-serif; padding: 40px; }
        h1 { text-align: center; }
        .chart-container { width: 700px; height: 400px; margin: 0 auto; }
    </style>
</head>
<body>
    <h1>Quarterly Sales</h1>
    <div class='chart-container'>
        <canvas id='chart'></canvas>
    </div>
    <script>
        new Chart(document.getElementById('chart'), {
            type: 'bar',
            data: {
                labels: ['Q1', 'Q2', 'Q3', 'Q4', 'Q5', 'Q6'],
                datasets: [{
                    label: 'Sales',
                    data: [100, 250, 180, 320, 280, 450],
                    backgroundColor: 'rgba(0, 102, 204, 0.8)'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: { legend: { display: false } }
            }
        });
    </script>
</body>
</html>";

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.EnableJavaScript = true;
renderer.RenderingOptions.WaitFor.JavaScript(2000);
renderer.RenderingOptions.PaperSize = PdfPaperSize.Custom;
renderer.RenderingOptions.SetCustomPaperSizeinPixelsOrPoints(800, 600);

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("chart.pdf");
```

### 例4: カスタムページサイズと形状

**VectSharp:**
```csharp
using VectSharp;
using VectSharp.PDF;

Document doc = new Document();
Page page = new Page(800, 600);
Graphics graphics = page.Graphics;

// 円を描画
GraphicsPath circlePath = new GraphicsPath();
circlePath.Arc(400, 300, 100, 0, 2 * Math.PI);
graphics.FillPath(circlePath, Colour.FromRgb(255, 0, 0));

// 円の中にテキストを追加
graphics.FillText(360, 295, "Circle",
    new Font(new FontFamily("Arial"), 16), Colours.White);

doc.Pages.Add(page);
doc.SaveAsPDF("custom.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

var html = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { margin: 0; }
        .container {
            width: 800px;
            height: 600px;
            display: flex;
            justify-content: center;
            align-items: center;
        }
        .circle {
            width: 200px;
            height: 200px;
            background-color: red;
            border-radius: 50%;
            display: flex;
            justify-content: center;
            align-items: center;
        }
        .circle span {
            color: white;
            font-family: Arial, sans-serif;
            font-size: 16px;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='circle'>
            <span>Circle</span>
        </div>
    </div>
</body>
</html>";

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.Custom;
renderer.RenderingOptions.SetCustomPaperSizeinPixelsOrPoints(800, 600);

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("custom.pdf");
```

### 例5: IronPDFでのSVGグラフィックス（両方の世界のベスト）

**VectSharp:**
```csharp
using VectSharp;
using VectSharp.PDF;

Document doc = new Document();
Page page = new Page(600, 400);
Graphics graphics = page.Graphics;

// 複雑なパス描画
GraphicsPath path = new GraphicsPath();
path.MoveTo(300, 50);
path.LineTo(400, 150);
path.LineTo(350, 250);
path.LineTo(250, 250);
path.LineTo(200, 150);
path.Close();

graphics.FillPath(path, Colour.FromRgb(0, 128, 0));
graphics.StrokePath(path, Colour.FromRgb(0, 100, 0), 3);

doc.Pages.Add(page);
doc.SaveAsPDF("polygon.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

// ベクターグラフィックスにインラインSVGを使用
var html = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { margin: 0; padding: 20px; }
    </style>
</head>
<body>
    <svg width='600' height='400' viewBox='0 0 600 400'>
        <polygon
            points='300,50 400,150 350,250 250,250 200,150'
            fill='rgb(0, 128, 0)'
            stroke='rgb(0, 100, 0)'
            stroke-width='3'/>
    </svg>
</body>
</html>";

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.Custom;
renderer.RenderingOptions.SetCustomPaperSizeinPixelsOrPoints(600, 400);

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("polygon.pdf");
```

### 例6: ビジネスレポート（IronPDFが優れている理由）

**VectSharp（非常に冗長）:**
```csharp
using VectSharp;
using VectSharp.PDF;

Document doc = new Document();
Page page = new Page(595, 842);
Graphics graphics = page.Graphics;

Font titleFont = new Font(new FontFamily("Arial"), 28);
Font headerFont = new Font(new FontFamily("Arial"), 14);
Font bodyFont = new Font(new FontFamily("Arial"), 11);

double y = 50;

// タイトル
graphics.FillText(50, y, "Monthly Sales Report", titleFont, Colours.Black);
y += 50;

// サブタイトル
graphics.FillText(50, y, $"Generated: {DateTime.Now:MMMM yyyy}", bodyFont, Colours.Gray);
y += 40;

// テーブルヘッダー
graphics.FillRectangle(50, y, 495, 25, Colour.FromRgb(240, 240, 240));
graphics.StrokeRectangle(50, y, 495, 25, Colours.Black, 1);
graphics.FillText(55, y + 7, "Product", headerFont, Colours.Black);
graphics.FillText(200, y + 7, "Q1", headerFont, Colours.Black);
graphics.FillText(280, y + 7, "Q2", headerFont, Colours.Black);
graphics.FillText(360, y + 7, "Q3", headerFont, Colours.Black);
graphics.FillText(440, y + 7, "Q4", headerFont, Colours.Black);
y += 25;

// テーブル行
string[,] data = {
    { "Widget A", "$12,000", "$14,500", "$11,800", "$16,200" },
    { "Widget B", "$8,500", "$9,200", "$10,100", "$11,000" },
    { "Widget C", "$5,200", "$4,800", "$6,100", "$7,300" }
};

for (int row = 0; row < 3; row++)
{
    graphics.StrokeRectangle(50, y, 495, 22, Colours.LightGray, 1);
    graphics.FillText(55, y + 5, data[row, 0], bodyFont, Colours.Black);
    graphics.FillText