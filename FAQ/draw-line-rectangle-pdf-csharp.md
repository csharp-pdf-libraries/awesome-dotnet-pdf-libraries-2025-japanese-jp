---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [FAQ/draw-line-rectangle-pdf-csharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/draw-line-rectangle-pdf-csharp.md)
🇯🇵 **日本語:** [FAQ/draw-line-rectangle-pdf-csharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/draw-line-rectangle-pdf-csharp.md)

---

# C#でPDFに線、長方形、形を描く方法は？

もちろんです！C#で直接PDFに描画することで、HTMLからPDFへの変換では実現できないピクセルパーフェクトなレイアウトやカスタムグラフィックスを実現できます。PDFに鮮明な線、ボックス、署名、またはカスタム形状を追加したい場合、HTMLテーブルやCSSの技巧に頼ることなく、このFAQがシンプルな線から高度な形状、パフォーマンスのヒントまで、実用的なステップをガイドします。

---

## なぜHTMLを使わずにPDFファイルに直接描画したいのですか？

HTMLからPDFへの変換は基本的なレイアウトには機能しますが、次のような場合には実際の制限があります：

- **正確なセパレーターやボーダー**が、コンテンツに関係なく完璧に揃うこと
- **色付きのボックスやボーダーでセクションを強調表示**すること
- **正確な視覚要件に合った署名ラインやチェックボックスなどのカスタムフォーム要素** 
- **グリッドラインとカスタムセル背景を持つピクセル精度のテーブル**
- **下線、ロゴフレーム、ウォーターマークなどのブランディングとグラフィック要素**

直接描画により、すべてのポイントと色を完全に制御でき、CSSのハックやHTMLの制約を回避できます。[IronPDF](https://ironpdf.com)は、暗号化されたPDF仕様に没頭することなく、これらを実現できるアプローチャブルなAPIを.NET開発者に提供します。

PDF構造の操作については、[C#でPDF DOMにアクセスして変更する方法は？](access-pdf-dom-object-csharp.md)を参照してください。

---

## C#でPDFに描画を始めるには何が必要ですか？

[IronPDF](https://ironpdf.com)ライブラリ（NuGet経由で利用可能）とC#の基本的な理解だけが必要です。

### IronPDFをインストールして設定するにはどうすればいいですか？

NuGet経由でプロジェクトにIronPDFを追加します：

```bash
Install-Package IronPdf
```

次に、必要な名前空間をインポートします：

```csharp
using IronPdf;
using IronSoftware.Drawing; // 高度な色と描画機能用
```

ビットマップ上に形状を描画してからPDFに配置する場合は、`System.Drawing`も参照してください。

> IronPDFは、.NET Frameworkおよび.NET Core/5/6/7+で動作します。

### PDFドキュメントを開いて、作成して、保存するにはどうすればいいですか？

既存のPDFを開いて描画の準備をするには：

```csharp
using IronPdf;
using IronSoftware.Drawing;

var pdfDoc = PdfDocument.FromFile("input.pdf");

// [ここで描画]

pdfDoc.SaveAs("output.pdf");
```

最初から新しいPDFを作成する場合（例えば、新しい空白ページ）：

```csharp
var pdfDoc = PdfDocument.Create();
pdfDoc.AddPage(); // 作業するための空白ページを追加
```

---

## C#でPDFに線を描くには？

線を描くことは、PDFグラフィック操作の中で最も一般的なものの一つであり、セパレーター、下線、フォーム要素に役立ちます。

### 線を描くための構文は何ですか？

IronPDFは`DrawLine`メソッドを提供します：

```csharp
pdfDoc.DrawLine(startX, startY, endX, endY, pageIndex, color, thickness);
```

例：最初のページの上部に横線を引く：

```csharp
pdfDoc.DrawLine(50, 750, 550, 750, 0, Color.Black, 2);
```

**覚えておいてください：**PDFの座標は左下から始まり、左上ではありません。したがって、Yを増やすと上に移動します。

[PDF座標での作業についての詳細は、配置セクションを参照してください。](#how-do-i-position-and-align-shapes-accurately-on-a-pdf-page)

### 縦、横、または斜めの線を描くことはできますか？

はい！座標を調整するだけです：

```csharp
// 横
pdfDoc.DrawLine(100, 700, 400, 700, 0, Color.Gray, 1);

// 縦
pdfDoc.DrawLine(200, 100, 200, 700, 0, Color.Blue, 2);

// 斜め
pdfDoc.DrawLine(150, 600, 450, 300, 0, Color.Red, 2);
```

### 線の色、太さ、または透明度を変更するにはどうすればいいですか？

色は`System.Drawing.Color`であり、カスタムRGBやアルファ（透明度のため）を含むことができます：

```csharp
var translucentPurple = Color.FromArgb(120, 128, 0, 128);
pdfDoc.DrawLine(100, 650, 400, 650, 0, translucentPurple, 4);
```

- **太さ**はポイント単位です（1ポイントあたり1/72インチ）。
- **アルファ（透明度）**は機能しますが、最適な結果を得るために複数のPDFビューアで出力をテストしてください。

より高度なフォントと色の扱いについては、[C#でPDFドキュメントのフォントと色を管理するにはどうすればいいですか？](manage-fonts-pdf-csharp.md)を参照してください。

### IronPDFは点線や点線をサポートしていますか？

IronPDFの組み込み線描画は常に実線です。点線については、ビットマップに描画してからその画像を埋め込んでください。こちらがその方法です：

```csharp
using System.Drawing;

using var bmp = new Bitmap(400, 20);
using var gfx = Graphics.FromImage(bmp);

var dashedPen = new Pen(System.Drawing.Color.DarkSlateGray, 3);
dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

gfx.DrawLine(dashedPen, 0, 10, 400, 10);

pdfDoc.DrawBitmap(bmp, 100, 600, 0);
```

**ヒント：**ビットマップ上であらゆる`System.Drawing`操作を使用できます。これにはパターンやカスタムエフェクトも含まれ、その結果をPDFに配置できます。

ビットマップとテキストの描画に関する専用ガイドについては、[C#でPDFにテキストとビットマップを描画するには？](draw-text-bitmap-pdf-csharp.md)を参照してください。

### 既存のPDFコンテンツの上に描画することはできますか？

はい—描画方法は、PDF内の既存のものの上にグラフィックを重ねます。これは、注釈、ハイライト、古い値の取り消し線に便利です。

```csharp
// 取り消し線
pdfDoc.DrawLine(120, 520, 380, 520, 0, Color.Red, 2);

// ハイライトバー（半透明の黄色）
var highlightYellow = Color.FromArgb(60, 255, 255, 0);
pdfDoc.DrawRectangle(110, 510, 260, 25, 0, highlightYellow, 0);
```

---

## PDFに長方形、ボーダー、ボックスを描くにはどうすればいいですか？

セクションのボーダー、ハイライト、フォームフィールドに最適な長方形を描画します。

### 長方形のボーダーを描く基本的な方法は何ですか？

```csharp
pdfDoc.DrawRectangle(100, 500, 400, 200, 0, Color.DarkCyan, 3);
```

- 長方形の左下隅は(100, 500)にあります
- 幅：400ポイント、高さ：200ポイント
- 3ポイントのボーダーでページ0に描画されます

### 塗りつぶされた長方形やカスタム背景を描くにはどうすればいいですか？

IronPDFの`DrawRectangle`はボーダーのみを描画します。塗りつぶされた長方形やカラーバックグラウンドには、まずビットマップに描画してから行います：

```csharp
using System.Drawing;

using var rectBmp = new Bitmap(400, 200);
using var gfx = Graphics.FromImage(rectBmp);

gfx.Clear(System.Drawing.Color.MistyRose); // 塗りつぶし

var borderPen = new Pen(System.Drawing.Color.IndianRed, 5);
gfx.DrawRectangle(borderPen, 0, 0, 399, 199);

pdfDoc.DrawBitmap(rectBmp, 100, 500, 0);
```

この技術を使用すると、グラデーション、テクスチャ、パターンなどの高度な背景も実現できます。

### 角の丸い長方形やボックスを描くにはどうすればいいですか？

角の丸い長方形を実現するには、GDI+のパス描画を使用し、形状をビットマップとして埋め込みます：

```csharp
using System.Drawing;
using System.Drawing.Drawing2D;

using var canvas = new Bitmap(400, 200);
using var gfx = Graphics.FromImage(canvas);

gfx.SmoothingMode = SmoothingMode.AntiAlias;
var rect = new Rectangle(10, 10, 380, 180);
int cornerRadius = 25;

using var path = new GraphicsPath();
path.AddArc(rect.Left, rect.Top, cornerRadius, cornerRadius, 180, 90);
path.AddArc(rect.Right - cornerRadius, rect.Top, cornerRadius, cornerRadius, 270, 90);
path.AddArc(rect.Right - cornerRadius, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 0, 90);
path.AddArc(rect.Left, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 90, 90);
path.CloseFigure();

using var orangePen = new Pen(Color.OrangeRed, 4);
gfx.DrawPath(orangePen, path);

pdfDoc.DrawBitmap(canvas, 100, 500, 0);
```

これは、コールアウト、署名ボックス、またはモダンなUIエフェクトに最適です。

### PDFページ全体にボーダーやフレームを追加するにはどうすればいいですか？

すべてのページをフレーム化するには（たとえば、レターヘッド用に）、すべてのページをループして、マージンを残してページの寸法に一致する長方形を描画します：

```csharp
for (int i = 0; i < pdfDoc.PageCount; i++)
{
    // レターサイズ：612x792ポイント、12ポイントのマージン付き
    pdfDoc.DrawRectangle(12, 12, 588, 768, i, Color.Black, 2);
}
```

このアプローチは、均一なページフレーミングと特定のセクションのマーキングの両方に機能します。

PDFページをプログラムでアクセスまたは操作したい場合は、[C#でPDF DOMにアクセスして変更する方法は？](access-pdf-dom-object-csharp.md)を参照してください。

---

## C#でPDFに円、楕円、カスタム形状を描くことはできますか？

もちろんです！IronPDFの組み込みメソッドは線と長方形に焦点を当てていますが、ビットマップに描画できる任意の形状をPDFに埋め込むことができます。

### 円や楕円を描くにはどうすればいいですか？

形状をビットマップに描画してから、PDFに挿入します：

```csharp
using System.Drawing;

using var circleBmp = new Bitmap(80, 80);
using var gfx = Graphics.FromImage(circleBmp);

gfx.Clear(System.Drawing.Color.Transparent);

var greenPen = new Pen(System.Drawing.Color.ForestGreen, 4);
gfx.DrawEllipse(greenPen, 2, 2, 76, 76);

pdfDoc.DrawBitmap(circleBmp, 200, 500, 0);
```

楕円の場合は幅と高さを変更します。

### 署名フィールド、チェックボックス、またはカスタムフォーム要素を描くにはどうすればいいですか？

署名フィールド（長方形、ラベル、ベースライン）の場合：

```csharp
int boxX = 130, boxY = 160;

pdfDoc.DrawRectangle(boxX, boxY, 280, 60, 0, Color.Black, 1);
pdfDoc.DrawText("Signature:", boxX, boxY + 75, 0);
pdfDoc.DrawLine(boxX + 10, boxY + 18, boxX + 270, boxY + 18, 0, Color.Gray, 1);
```

チェックボックスの場合は、必要に応じて小さな正方形や円を描画します。

テキストの描画と配置については、[C#でPDFにテキストとビットマップを描画するには？](draw-text-bitmap-pdf-csharp.md)を参照してください。

### 正確に揃ったカスタムテーブルグリッドを手動で作成するにはどうすればいいですか？

手動で完璧に揃ったテーブルグリッドラインを作成するには：

```csharp
int tableX = 90, tableY = 650, tableWidth = 420, tableHeight = 280;
int rowCount = 7, colCount = 4;

// 水平線
for (int i = 0; i <= rowCount; i++)
{
    int yLine = tableY + (i * tableHeight