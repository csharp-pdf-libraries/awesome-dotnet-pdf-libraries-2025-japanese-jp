# C# と IronPDF を使用して PDF に線や四角を描く方法は？

C# でプログラム的に線、ボックス、または注釈を PDF に追加する必要がありますか？IronPDF を使用すると、文書に直接描画することが簡単になります。これは、署名フィールド、図、グリッドなどに最適です。この FAQ では、セットアップから高度なヒントまで、IronPDF を使用した描画の実用的なシナリオをカバーしています。

## なぜ PDF に直接描画したいのですか？

HTML から PDF への変換だけでは柔軟性に欠ける場合があります。PDF に直接描画することで、ハイライト、ボックス、図、注釈を正確に配置できます。これは、以下のような機能に理想的です：
- フォーム上の署名フィールド
- 組織図や図表
- カスタムボーダーやセパレーター
- 重要なコンテンツのハイライト
- 方眼紙やテーブルレイアウトの作成

PDF 構造を操作するための詳細な情報については、[C# で PDF DOM オブジェクトにアクセスする方法は？](access-pdf-dom-object-csharp.md) を参照してください。

## IronPDF を設定して PDF に描画するにはどうすればよいですか？

NuGet 経由で IronPDF の設定は簡単です：

```csharp
// Install-Package IronPdf
```

次に、C# ファイルの先頭にこれらの using ステートメントを追加します：

```csharp
using IronPdf;
using IronPdf.Drawing;
using System.Drawing;
```

これで、描画を開始する準備が整いました！

## PDF の座標系はどのように機能しますか？

PDF は左上原点を使用します。すべての座標はポイントで測定されます（1 インチ = 72 ポイント）。例えば、レターサイズは 612x792 ポイント、A4 は 595x842 ポイントです。線やボックスを正確に配置するには、これらの単位を使用して精度を上げます。

## C# で PDF に線を描くにはどうすればよいですか？

線を描くのは、始点と終点、色、幅を指定するだけの簡単な作業です：

```csharp
using IronPdf;
using IronPdf.Drawing;
using System.Drawing;
// Install-Package IronPdf

var doc = new PdfDocument("input.pdf");
doc.DrawLine(new PointF(50, 200), new PointF(550, 200), Color.Black, 1);
doc.SaveAs("output-with-line.pdf");
```

任意の色や太さを設定したり、任意の角度で描画したりできます。高度な線描画（テキストオーバーレイを含む）については、[C# で PDF にテキストやビットマップを描画する方法は？](draw-text-bitmap-pdf-csharp.md) をチェックしてください。

### 線のスタイルをカスタマイズしたり、複数ページに描画したりすることは可能ですか？

IronPDF は現在、実線のみをサポートしていますが、色と幅を調整できます：

```csharp
doc.DrawLine(new PointF(50, 100), new PointF(550, 100), Color.Gray, 2);
```

特定のページに描画するには、`pageIndex` パラメーター（ゼロベース）を使用します：

```csharp
doc.DrawLine(new PointF(60, 60), new PointF(500, 60), Color.Red, 2, pageIndex: 0); // 1 ページ目
```

点線や破線が必要な場合は、以下の一般的な問題セクションでの回避策を参照してください。

## 四角形、ボーダー、またはハイライトを描画するにはどうすればよいですか？

四角形を描画するのも直接的です：

```csharp
var doc = new PdfDocument("form.pdf");
doc.DrawRectangle(new RectangleF(50, 100, 200, 80), Color.Black, 1);
doc.SaveAs("form-box.pdf");
```

四角形は、指定された幅と高さ（ポイント単位）で左上隅から始まります。

### 塗りつぶされた四角形やハイライトを描画することはできますか？

IronPDF の `DrawRectangle` はデフォルトでアウトラインを描画しますが、薄い線を重ねることで塗りつぶしボックス（ハイライトや塗りつぶしに便利）をシミュレートできます：

```csharp
var fillRect = new RectangleF(100, 200, 300, 40);
var highlight = Color.FromArgb(120, 255, 255, 0); // 半透明の黄色
for (float y = fillRect.Top; y < fillRect.Bottom; y++)
{
    doc.DrawLine(new PointF(fillRect.Left, y), new PointF(fillRect.Right, y), highlight, 1);
}
doc.DrawRectangle(fillRect, Color.Orange, 2); // ボーダー
doc.SaveAs("highlighted.pdf");
```

ボックス内にテキストラベルを追加したい場合は、[C# で PDF にテキストやビットマップを描画する方法は？](draw-text-bitmap-pdf-csharp.md) を参照してください。

## 署名ボックスを追加したり、PDF に注釈を付けたりするにはどうすればよいですか？

署名ボックスは一般的な使用例です。ラベル付きの署名フィールドを作成する方法は次のとおりです：

```csharp
int lastPage = doc.PageCount - 1;
var boxRect = new RectangleF(50, 650, 250, 60);
doc.DrawRectangle(boxRect, Color.Black, 1, lastPage);

// 署名用の線を描画
float lineY = boxRect.Bottom - 15;
doc.DrawLine(new PointF(boxRect.Left + 10, lineY), new PointF(boxRect.Right - 10, lineY), Color.Black, 0.5f, lastPage);
doc.SaveAs("signature-box.pdf");
```

カスタムテキストや指示を追加するには、[C# で PDF にテキストやビットマップを描画する方法は？](draw-text-bitmap-pdf-csharp.md) に示されている `DrawText` と組み合わせます。

## PDF ページにボーダーやグリッドを描画するにはどうすればよいですか？

証明書に最適なページボーダーを描画するには：

```csharp
float margin = 30;
float width = doc.PageSizes[0].Width, height = doc.PageSizes[0].Height;
var border = new RectangleF(margin, margin, width - 2 * margin, height - 2 * margin);
doc.DrawRectangle(border, Color.DarkBlue, 3);
doc.SaveAs("bordered.pdf");
```

カスタムグリッド（方眼紙のような）には：

```csharp
float startX = 50, startY = 50, gridW = 500, gridH = 700, cell = 20;
for (float x = startX; x <= startX + gridW; x += cell)
    doc.DrawLine(new PointF(x, startY), new PointF(x, startY + gridH), Color.LightGray, 0.5f);
for (float y = startY; y <= startY + gridH; y += cell)
    doc.DrawLine(new PointF(startX, y), new PointF(startX + gridW, y), Color.LightGray, 0.5f);
doc.DrawRectangle(new RectangleF(startX, startY, gridW, gridH), Color.Black, 1);
doc.SaveAs("grid.pdf");
```

グリッドやレイアウトのアイデアについては、[C# で PDF に線や四角を描画する方法は？](draw-line-rectangle-pdf-csharp.md) を参照してください。

## カスタムカラーや透明度を使用することはできますか？

もちろんです！`System.Drawing.Color`（透明度を含むアルファ）を含む任意の色がサポートされています：

```csharp
var brandBlue = Color.FromArgb(0, 122, 204);
var transparentBlack = Color.FromArgb(100, 0, 0, 0);
doc.DrawRectangle(new RectangleF(50, 100, 200, 80), brandBlue, 3);
doc.DrawRectangle(new RectangleF(100, 400, 200, 100), transparentBlack, 1);
doc.SaveAs("custom-colors.pdf");
```

すべての PDF ビューアが透明度を同じように扱うわけではないので、複数のプラットフォームでテストしてください。透かしのヒントについては、[この透かしガイド](https://ironpdf.com/how-to/custom-watermark/) を参照してください。

## PDF に描画する際の一般的な問題は何ですか？

ここには、典型的な問題と解決策があります：

### 私の描画が間違った場所に表示されるのはなぜですか？

PDF の原点は左上です。座標とページサイズが期待に合っていることを確認してください。レイアウト制御の詳細については、[C# で PDF DOM オブジェクトにアクセスする方法は？](access-pdf-dom-object-csharp.md) を参照してください。

### 点線や破線を作成するにはどうすればよいですか？

IronPDF はネイティブに点線をサポートしていませんが、短いセグメントをループで描画することで模倣できます：

```csharp
float dash = 10, gap = 5, x1 = 60, x2 = 200, y = 300;
for (float x = x1; x < x2; x += dash + gap)
{
    float end = Math.Min(x + dash, x2);
    doc.DrawLine(new PointF(x, y), new PointF(end, y), Color.Black, 1);
}
```

### 既存のコンテンツを注釈が覆ってしまう場合はどうすればよいですか？

描画はページの上にレイヤーされます。ハイライトには透明度を使用するか、重なりを避けるために位置を調整してください。高度な注釈については、[C# で PDF にテキストやビットマップを描画する方法は？](draw-text-bitmap-pdf-csharp.md) を参照してください。

### 多くの描画でパフォーマンスに影響はありますか？

形状を何百も描画すると、処理が遅くなることがあります。バッチ操作を行い、不必要な再描画を避けてください。非常に複雑なレイアウトの場合は、ビットマップにレンダリングしてその画像を埋め込むことを検討してください。

## さらに学ぶには、または助けを得るにはどこに行けばよいですか？

より多くの描画技術については、[C# で PDF に線や四角を描画する方法は？](draw-line-rectangle-pdf-csharp.md) をチェックするか、完全なドキュメントとツールについては [IronPDF](https://ironpdf.com) を訪問してください。グリッド間隔などの数値を操作する必要がある場合は、[C# で数字を小数点以下 2 桁に丸める方法は？](csharp-round-to-2-decimal-places.md) を参照してください。PDF が大きい場合は、[C# で PDF を圧縮する方法は？](pdf-compression-csharp.md) を参照してください。

---

*[Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/)、[Iron Software](https://ironsoftware.com) の CTO。2017 年以来、開発者の生活を楽にするツールを構築しています。*
---

## 関連リソース

### 📚 チュートリアル & ガイド
- **[HTML から PDF へのガイド](../html-to-pdf-csharp.md)** — 完全な変換チュートリアル
- **[2025 年のベスト PDF ライブラリ](../best-pdf-libraries-dotnet-2025.md)** — ライブラリ比較
- **[初心者チュートリアル](../csharp-pdf-tutorial-beginners.md)** — 最初の PDF を 5 分で
- **[決定フローチャート](../choosing-a-pdf-library.md)** — 適切なライブラリを見つける

### 🔧 PDF 操作
- **[PDF のマージ & 分割](../merge-split-pdf-csharp.md)** — 文書の結合
- **[デジタル署名](../digital-signatures-pdf-csharp.md)** — 法的に署名
- **[テキストの抽出](../extract-text-from-pdf-csharp.md)** — テキスト抽出
- **[PDF フォームの入力](../fill-pdf-forms-csharp.md)** — フォーム自動化

### 🚀 フレームワーク統合
- **[ASP.NET Core PDF](../asp-net-core-pdf-reports.md)** — Web アプリ統合
- **[Blazor PDF 生成](../blazor-pdf-generation.md)** — Blazor サポート
- **[クロスプラットフォーム展開](../cross-platform-pdf-dotnet.md)** — Docker、Linux、クラウド

### 📖 ライブラリドキュメント
- **[IronPDF](../ironpdf/)** — 完全な Chromium レンダリング
- **[QuestPDF](../questpdf/)** — コードファースト生成
- **[PDFSharp](../pdfsharp/)** — オープンソースオプション

---

*2025 年の [Awesome .NET PDF Libraries](../README.md) コレクションの一部 — 73 の C#/.NET PDF ライブラリを比較し、167 の FAQ 記事を提供しています。*

---

[Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmell