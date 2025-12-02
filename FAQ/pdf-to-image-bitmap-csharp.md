# C#を使用してPDFページを画像に変換する方法は？

C#でPDFを画像に変換することは、アプリのプレビュー、サムネイル、ユニバーサルシェアリングにおいて一般的なニーズです。[IronPDF](https://ironpdf.com)を使用すると、C#プロジェクトで直接、PDFページをPNG、JPEG、TIFFなどに簡単に変換できます。このFAQでは、PDFから画像への変換を統合する際に開発者が直面する最も実用的な質問とコード例をカバーしています。

---

## なぜC#でPDFを画像に変換するのですか？

PDFはブラウザーやアプリでプレビューするのが難しい場合がありますが、画像はユニバーサルにサポートされています。PDFを画像に変換することで、信頼性の高いドキュメントプレビューを保証し、ギャラリー用のサムネイルを生成し、さらなる画像処理（OCRなど）を簡素化できます。これは、ダッシュボード、検索結果、ブラウザープラグインの問題を避けたい任意のコンテキストに特に有用です。

画像をPDFに変換する方法に興味がある場合は、[C#で画像をPDFに変換する方法は？](image-to-pdf-csharp.md)のガイドをご覧ください。

---

## C#でPDFをPNG（または他の画像）に変換するにはどうすればいいですか？

[IronPDF](https://ironpdf.com)を使用すると、PDFを画像に変換するのは簡単です。こちらが簡単な例です：

```csharp
using IronPdf; // Install-Package IronPdf

var doc = PdfDocument.FromFile("input.pdf");
// "page-0.png", "page-1.png", etc. を出力します。
doc.RasterizeToImageFiles("page-*.png");
```

`*`ワイルドカードは、出力される各画像を自動で番号付けします。必要に応じて、ストリームやバイト配列からPDFを読み込むこともできます。

---

## サポートされている画像フォーマットと、それぞれをいつ使用するべきかは？

IronPDFは複数の出力フォーマットをサポートしています：

- **PNG：** 高品質、透明性をサポート - ウェブに最適。
- **JPEG：** ファイルサイズが小さい、サムネイルに適していますが、透明性はありません。
- **TIFF：** マルチページサポート、アーカイブやワークフローに理想的。
- **GIF/BMP：** 特定のレガシーなニーズにのみ使用 - 稀です。

フォーマットを指定するには、コード内のファイル拡張子を変更するだけです：

```csharp
doc.RasterizeToImageFiles("thumb-*.jpg"); // JPEG出力
doc.RasterizeToImageFiles("archive-*.tiff"); // TIFF出力
```

XMLやXAMLからPDFに変換する必要がある場合は、[C#でXMLをPDFに変換する方法は？](xml-to-pdf-csharp.md)と[C# MAUIでXAMLをPDFに変換する方法は？](xaml-to-pdf-maui-csharp.md)をチェックしてください。

---

## 画像の品質とDPIをどのように調整しますか？

画像の品質は主にDPI（ドットパーインチ）に依存します。DPIが高いほど画像は鮮明になりますが、ファイルサイズも大きくなります。IronPDFでDPIを設定する方法は以下の通りです：

```csharp
using IronPdf.Rendering;

doc.RasterizeToImageFiles("hires-*.png", new ImageRenderingOptions
{
    Dpi = 300 // 印刷品質
});
```

- **72 DPI：** ウェブ/スクリーンプレビューのデフォルト
- **150 DPI：** サムネイルに適しています
- **300 DPI：** 印刷品質

用途に応じてDPIを調整してください。

---

## 特定のPDFページのみを画像に変換することは可能ですか？

もちろんです。個々のページや範囲を対象にできます：

```csharp
doc.RasterizeToImageFiles("selected-*.png", new ImageRenderingOptions
{
    PageIndexes = new[] { 0, 2 } // ゼロベース：ページ1と3
});
```

特定のページのプレビューのみが必要な場合、これによりメモリを節約し、処理を速くすることができます。

---

## ファイルではなくメモリ内で画像を扱うにはどうすればいいですか？

画像をメモリ内で処理する（描画、透かし、埋め込みなど）場合は、`Bitmap`オブジェクトを生成します：

```csharp
using System.Drawing;
var bitmap = doc.ToBitmap(0);
// Graphicsを使用して注釈を付けたり、オーバーレイを描画できます
using (var g = Graphics.FromImage(bitmap))
{
    g.DrawString("DEMO", new Font("Arial", 36), Brushes.Red, 50, 50);
}
bitmap.Save("watermarked.png");
```

PDFにテキストや画像の透かしを追加する方法については、[C#でPDFに透かしやスタンプを追加する方法は？](stamp-text-image-pdf-csharp.md)をご覧ください。

---

## 画像サイズをカスタマイズしたり、サムネイルを作成することは可能ですか？

ギャラリー用の画像の出力寸法を自由に指定できます：

```csharp
doc.RasterizeToImageFiles("thumb-*.png", new ImageRenderingOptions
{
    Width = 200,
    Height = 260
});
```

これは、高速読み込みのギャラリープレビューやダッシュボードタイルに最適です。

---

## PDFからマルチページTIFFを作成することは可能ですか？

はい。マルチページTIFFは、アーカイブやドキュメント管理システムによく必要とされます。以下の方法で行います：

```csharp
using IronSoftware.Drawing;

doc.RasterizeToImageFiles("archive.tiff", new ImageRenderingOptions
{
    ImageType = AnyBitmap.ImageFormat.Tiff
});
```

すべてのPDFページが1つのTIFFファイルに結合されます。

---

## 多数のPDFを画像に一括変換するにはどうすればいいですか？

PDFのフォルダ全体を処理する場合は、ファイルをループしてそれぞれをラスタライズします：

```csharp
using System.IO;

foreach (var file in Directory.GetFiles("input", "*.pdf"))
{
    var pdf = PdfDocument.FromFile(file);
    var name = Path.GetFileNameWithoutExtension(file);
    pdf.RasterizeToImageFiles($"output/{name}-*.png");
}
```

大量のバッチ処理の場合は、より高速な処理のために`Parallel.ForEach`を使用します。高度なレンダリングの調整については、[IronPDFで利用可能なレンダリングオプションは何ですか？](pdf-rendering-options-csharp.md)をご覧ください。

---

## HTMLやAPI用にBase64画像が必要な場合はどうすればいいですか？

ファイルI/Oをスキップして、Base64で画像を直接埋め込むことができます：

```csharp
using System.IO;
using System.Drawing;

var bmp = doc.ToBitmap(0);
using var ms = new MemoryStream();
bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
string base64 = Convert.ToBase64String(ms.ToArray());
string dataUri = $"data:image/png;base64,{base64}";
// <img src="...">やJSONで使用
```

---

## パフォーマンスとファイルサイズを最適化するにはどうすればいいですか？

- **メモリ：** 一度に1ページを処理し、保存後にビットマップを破棄します。
- **速度：** フォルダの並列処理を使用します。
- **圧縮：** 小さいファイルのためにJPEGを使用するか、ImageSharpを介してカスタム圧縮を持つPNGを使用します。
- **レンダリングの問題：** 画像がぼやけている場合やフォントが正しくない場合は、IronPDFを更新して、より高いDPIで試してください。

---

## PDFから埋め込まれた画像のみを抽出することは可能ですか？

はい—IronPDFを使用すると、埋め込まれた画像アセット（ページ全体ではなく）を抽出することができます：

```csharp
var images = doc.ExtractAllImages();
foreach (var img in images)
{
    img.SaveAs($"extracted-{img.Width}x{img.Height}.png");
}
```

これは、署名、ロゴ、または任意の埋め込まれたグラフィックに便利です。

---

## 透明性、オーバーレイ、または透かしを追加するにはどうすればいいですか？

透明性を保持または追加するには、常にPNG出力を使用してください。オーバーレイや透かしを追加するには、ラスタライズ前にPDFを変更します：

```csharp
doc.DrawText("CONFIDENTIAL", 250, 400, 0);
doc.RasterizeToImageFiles("wm-*.png");
```

公式ドキュメントの[透かし](https://ironpdf.com/how-to/rasterize-pdf-to-images/)に関するさらなるガイダンスを参照してください。

---

## より多くのヘルプや高度な例をどこで見つけることができますか？

[IronPDFドキュメント](https://ironpdf.com/docs/)や[Iron Software](https://ironsoftware.com)サイトで詳細なガイドやサポートをチェックしてください。複雑なレンダリングを扱っている場合は、[IronPDFで利用可能なレンダリングオプションは何ですか？](pdf-rendering-options-csharp.md)を参照してください。

---

*著者について：[Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/)はIronPDFを創設し、現在[Iron Software](https://ironsoftware.com)でCTOを務めています。Jacobは6歳でコーディングを始め、実際のPDFの課題を解決するためにIronPDFを作成しました。タイのチェンマイに拠点を置いています。[LinkedIn](https://www.linkedin.com/in/jacob-mellor-iron-software/) | [GitHub](https://github.com/jacob-mellor)*

---

## 関連リソース

### 📚 チュートリアル & ガイド
- **[HTMLからPDFへのガイド](../html-to-pdf-csharp.md)** — 完全な変換チュートリアル
- **[2025年のベストPDFライブラリ](../best-pdf-libraries-dotnet-2025.md)** — ライブラリ比較
- **[初心者チュートリアル](../csharp-pdf-tutorial-beginners.md)** — 最初のPDFを5分で
- **[意思決定フローチャート](../choosing-a-pdf-library.md)** — 適切なライブラリを見つける

### 🔧 PDF操作
- **[PDFのマージ & 分割](../merge-split-pdf-csharp.md)** — ドキュメントの結合
- **[デジタル署名](../digital-signatures-pdf-csharp.md)** — 法的に署名
- **[テキストの抽出](../extract-text-from-pdf-csharp.md)** — テキスト抽出
- **[PDFフォームの入力](../fill-pdf-forms-csharp.md)** — フォーム自動化

### 🚀 フレームワーク統合
- **[ASP.NET Core PDF](../asp-net-core-pdf-reports.md)** — Webアプリ統合
- **[Blazor PDF生成](../blazor-pdf-generation.md)** — Blazorサポート
- **[クロスプラットフォーム展開](../cross-platform-pdf-dotnet.md)** — Docker、Linux、Cloud

### 📖 ライブラリドキュメント
- **[IronPDF](../ironpdf/)** — 完全なChromiumレンダリング
- **[QuestPDF](../questpdf/)** — コードファースト生成
- **[PDFSharp](../pdfsharp/)** — オープンソースオプション

---

*「[Awesome .NET PDF Libraries 2025](../README.md)」コレクションの一部 — 73のC#/.NET PDFライブラリを比較し、167のFAQ記事を提供しています。*

---

著者：[Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/)、Iron Softwareの最高技術責任者（CTO）。Jacobは6歳でコーディングを始め、実際のPDFの課題を解決するためにIronPDFを作成しました。タイのチェンマイに拠点を置いています。[LinkedIn](https://www.linkedin.com/in/jacob-mellor-iron-software/) | [GitHub](https://github.com/jacob-mellor)