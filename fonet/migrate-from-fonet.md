---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [fonet/migrate-from-fonet.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/fonet/migrate-from-fonet.md)
🇯🇵 **日本語:** [fonet/migrate-from-fonet.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/fonet/migrate-from-fonet.md)

---

# 移行ガイド: FoNet (FO.NET) → IronPDF

## 目次
1. [IronPDFへの移行理由](#ironpdfへの移行理由)
2. [開始前に](#開始前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [XSL-FOからHTMLへの変換ガイド](#xsl-foからhtmlへの変換ガイド)
6. [コード例](#コード例)
7. [高度なシナリオ](#高度なシナリオ)
8. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
9. [トラブルシューティング](#トラブルシューティング)
10. [移行チェックリスト](#移行チェックリスト)

---

## IronPDFへの移行理由

### FoNet (FO.NET) の課題

FoNetはXSL-FOからPDFへのレンダラーで、現代の開発において重大な制限があります:

1. **旧式の技術**: XSL-FO (Extensible Stylesheet Language Formatting Objects) は2001年のW3C仕様で、2006年以降更新されておらず、大部分が旧式とみなされています
2. **学習曲線が急**: XSL-FOは複雑なXMLベースのマークアップと特殊なフォーマットオブジェクト(fo:block, fo:table, fo:page-sequenceなど)の学習を要求します
3. **HTML/CSSのサポートなし**: HTMLやCSSをレンダリングできず、HTMLからXSL-FOマークアップへの手動変換が必要です
4. **放棄/保守されていない**: 元のCodePlexリポジトリは機能しておらず、GitHubのフォークもアクティブに保守されていません
5. **Windowsのみ**: FoNetはSystem.Drawingに内部依存があり、Linux/macOSで動作しません
6. **限られた現代の機能**: JavaScriptのサポートなし、CSS3なし、フレックスボックス/グリッドなし、現代のウェブフォントなし
7. **URLレンダリング不可**: Webページを直接レンダリングできず、HTMLからXSL-FOへの手動変換が必要です

### IronPDFの利点

| 項目 | FoNet (FO.NET) | IronPDF |
|------|---------------|---------|
| 入力フォーマット | XSL-FO (旧式のXML) | HTML/CSS (現代のウェブ標準) |
| 学習曲線 | 急 (XSL-FOの専門知識) | 緩やか (HTML/CSSの知識) |
| 保守 | 放棄された | 毎月アクティブに保守されている |
| プラットフォームサポート | Windowsのみ | 真のクロスプラットフォーム |
| CSSサポート | なし | 完全なCSS3 (Flexbox, Grid) |
| JavaScript | なし | 完全なJavaScriptサポート |
| URLレンダリング | サポートされていない | 組み込み |
| 現代の機能 | 限られている | ヘッダー、フッター、ウォーターマーク、セキュリティ |
| ドキュメント | 時代遅れ | 包括的なチュートリアル |

### スイッチが理にかなっている理由

FoNetは、XSL-FOがドキュメントフォーマットの標準になると予想されて設計されました。それは起こりませんでした。HTML/CSSが普遍的なドキュメントフォーマットになりました:

- **開発者の98%以上**がHTML/CSSを知っています
- **開発者の1%未満**がXSL-FOを知っています
- ほとんどのXSL-FOリソースは2005-2010年のものです

IronPDFを使えば、すでに持っているスキルを使ってプロフェッショナルなPDFを作成できます。

---

## 開始前に

### 前提条件

1. **.NET環境**: IronPDFは.NET Framework 4.6.2+、.NET Core 3.1+、.NET 5/6/7/8/9+をサポートしています
2. **NuGetアクセス**: NuGetからパッケージをインストールできることを確認してください
3. **ライセンスキー**: 本番用途でのIronPDFライセンスキーを取得してください

### プロジェクトのバックアップ

```bash
# バックアップブランチを作成
git checkout -b pre-ironpdf-migration
git add .
git commit -m "FoNetからIronPDFへの移行前のバックアップ"
```

### FoNetの使用箇所の特定

```bash
# すべてのFoNet参照を検索
grep -r "FonetDriver\|Fonet\|\.fo\"\|xsl-region" --include="*.cs" --include="*.csproj" .

# すべてのXSL-FOテンプレートファイルを検索
find . -name "*.fo" -o -name "*.xslfo" -o -name "*xsl-fo*"
```

### XSL-FOテンプレートのドキュメント化

移行前に、すべてのXSL-FOファイルをカタログ化してください:
- ページの寸法とマージン
- 使用されているフォント
- テーブルとその構造
- ヘッダーとフッター (fo:static-content)
- ページ番号のパターン
- 画像参照

---

## クイックスタート移行

### ステップ1: NuGetパッケージの更新

```bash
# FoNetパッケージを削除
dotnet remove package Fonet
dotnet remove package FO.NET

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2: 名前空間の更新

```csharp
// 以前
using Fonet;
using Fonet.Render.Pdf;
using System.Xml;

// 以後
using IronPdf;
using IronPdf.Rendering;
```

### ステップ3: IronPDFの初期化

```csharp
// アプリケーション起動時にライセンスキーを設定
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### ステップ4: 基本的な変換パターン

```csharp
// 以前 (FoNetでXSL-FOを使用)
FonetDriver driver = FonetDriver.Make();
using (FileStream output = new FileStream("output.pdf", FileMode.Create))
{
    driver.Render(new StringReader(xslFoContent), output);
}

// 以後 (IronPDFでHTMLを使用)
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(htmlContent);
pdf.SaveAs("output.pdf");
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| FoNet 名前空間 | IronPDF相当 | 備考 |
|-----------------|-------------------|-------|
| `Fonet` | `IronPdf` | メイン名前空間 |
| `Fonet.Render.Pdf` | `IronPdf` | PDFレンダリング |
| `Fonet.Layout` | N/A | レイアウトはCSSによって処理される |
| `Fonet.Fo` | N/A | フォーマットオブジェクト → HTML |
| `Fonet.Datatypes` | N/A | C#の型を使用 |
| `Fonet.Image` | `IronPdf` | 画像処理組み込み |

### FonetDriverからChromePdfRendererへ

| FonetDriverメソッド | IronPDF相当 | 備考 |
|--------------------|-------------------|-------|
| `FonetDriver.Make()` | `new ChromePdfRenderer()` | レンダラーを作成 |
| `driver.Render(inputStream, outputStream)` | `renderer.RenderHtmlAsPdf(html)` | ストリームベース |
| `driver.Render(xmlReader, outputStream)` | `renderer.RenderHtmlAsPdf(html)` | XMLリーダーは不要 |
| `driver.Render(xmlDoc, outputStream)` | `renderer.RenderHtmlAsPdf(html)` | XMLドキュメントは不要 |
| `driver.Render(inputFile, outputStream)` | `renderer.RenderHtmlFileAsPdf(path)` | ファイルベース |
| `driver.OnError += handler` | Try/catchでレンダリングを囲む | エラー処理 |
| `driver.OnWarning += handler` | IronPDFのログ | 警告処理 |
| `driver.OnInfo += handler` | N/A | 情報ログ |

### FonetDriverプロパティ

| FonetDriverプロパティ | IronPDF相当 | 備考 |
|---------------------|-------------------|-------|
| `driver.CloseOnExit` | 自動 | IronPDFがリソースを処理 |
| `driver.Options` | `RenderingOptions` | 設定 |
| `driver.BaseDirectory` | `RenderingOptions.BaseUrl` | リソースのベースパス |

### RenderingOptions (PDF設定)

| FoNet (XSL-FO属性) | IronPDF RenderingOptions | 備考 |
|--------------------------|-------------------------|-------|
| `page-height` | `PaperSize`または`SetCustomPaperSize()` | ページ寸法 |
| `page-width` | `PaperSize` | 標準またはカスタム |
| `margin-top` | `MarginTop` | ミリメートル単位 |
| `margin-bottom` | `MarginBottom` | ミリメートル単位 |
| `margin-left` | `MarginLeft` | ミリメートル単位 |
| `margin-right` | `MarginRight` | ミリメートル単位 |
| `reference-orientation` | `PaperOrientation` | 縦/横 |

### XSL-FO要素からHTML/CSSへ

| XSL-FO要素 | HTML/CSS相当 | 備考 |
|----------------|-------------------|-------|
| `<fo:root>` | `<html>` | ルート要素 |
| `<fo:layout-master-set>` | CSS `@page` ルール | ページ設定 |
| `<fo:simple-page-master>` | CSS `@page` | ページ定義 |
| `<fo:page-sequence>` | `<body>`または`<div>` | ページコンテンツ |
| `<fo:flow>` | `<main>`または`<div>` | メインコンテンツエリア |
| `<fo:static-content>` | `HtmlHeaderFooter` | ヘッダー/フッター |
| `<fo:block>` | `<p>`, `<div>`, `<h1>-<h6>` | ブロックコンテンツ |
| `<fo:inline>` | `<span>` | インラインコンテンツ |
| `<fo:table>` | `<table>` | テーブル |
| `<fo:table-row>` | `<tr>` | テーブル行 |
| `<fo:table-cell>` | `<td>`, `<th>` | テーブルセル |
| `<fo:list-block>` | `<ul>`, `<ol>` | リスト |
| `<fo:list-item>` | `<li>` | リストアイテム |
| `<fo:external-graphic>` | `<img>` | 画像 |
| `<fo:page-number>` | `{page}` プレースホルダー | ページ番号 |
| `<fo:page-number-citation>` | `{total-pages}` | 総ページ数 |
| `<fo:leader>` | CSS `flex: 1` またはドットパターン | リーダー/タブ |
| `<fo:footnote>` | HTML脚注パターン | 脚注 |
| `<fo:basic-link>` | `<a href>` | ハイパーリンク |

### XSL-FOプロパティからCSSへ

| XSL-FOプロパティ | CSS相当 | 例 |
|-----------------|---------------|---------|
| `font-family` | `font-family` | 同じ構文 |
| `font-size` | `font-size` | 同じ構文 |
| `font-weight` | `font-weight` | `bold`, `normal`, `700` |
| `font-style` | `font-style` | `italic`, `normal` |
| `text-align` | `text-align` | `left`, `center`, `right`, `justify` |
| `color` | `color` | Hex, RGB, 名前 |
| `background-color` | `background-color` | 同じ構文 |
| `border` | `border` | 同じ構文 |
| `padding` | `padding` | 同じ構文 |
| `margin` | `margin` | 同じ構文 |
| `space-before` | `margin-top` | 要素の前 |
| `space-after` | `margin-bottom` | 要素の後 |
| `start-indent` | `margin-left` | 左インデント |
| `end-indent` | `margin-right` | 右インデント |
| `line-height` | `line-height` | 同じ構文 |
| `text-indent` | `text-indent` | 同じ構文 |
| `text-decoration` | `text-decoration` | `underline`, `none` |
| `vertical-align` | `vertical-align` | 同じ構文 |
| `display-align` | `vertical-align` | テーブルセル内 |
| `keep-together` | `page-break-inside: avoid` | 分割を防ぐ |
| `break-before="page"` | `page-break-before: always` | ページブレークを強制 |
| `break-after="page"` | `page-break-after: always` | ページブレークを強制 |
| `width` | `width` | 同じ構文 |
| `height` | `height` | 同じ構文 |
| `content-width` | `width` (画像に対して) | 画像の幅 |
| `content-height` | `height` (画像に対して) | 画像の高さ |

---

## XSL-FOからHTMLへの変換ガイド

### ページ設定

**XSL-FO:**
```xml
<fo:layout-master-set>
    <fo:simple-page-master master-name="A4"
        page-height="297mm" page-width="210mm"
        margin-top="20mm" margin-bottom="20mm"
        margin-left="25mm" margin-right="25mm">
        <fo:region-body margin-top="30mm" margin-bottom="30mm"/>
        <fo:region-before extent="25mm"/>
        <fo:region-after extent="25mm"/>
    </fo:simple-page-master>
</fo:layout-master-set>
```

**HTML/CSS + IronPDF:**
```csharp
var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.MarginTop = 20;
renderer.RenderingOptions.MarginBottom = 20;
renderer.RenderingOptions.MarginLeft = 25;
renderer.Rendering