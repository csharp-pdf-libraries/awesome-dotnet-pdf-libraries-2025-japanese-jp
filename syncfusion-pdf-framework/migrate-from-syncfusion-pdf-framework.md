---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [syncfusion-pdf-framework/migrate-from-syncfusion-pdf-framework.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/syncfusion-pdf-framework/migrate-from-syncfusion-pdf-framework.md)
🇯🇵 **日本語:** [syncfusion-pdf-framework/migrate-from-syncfusion-pdf-framework.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/syncfusion-pdf-framework/migrate-from-syncfusion-pdf-framework.md)

---

# Syncfusion PDF フレームワークから IronPDF への移行方法は？

Syncfusion PDF フレームワークは、Essential Studio スイートの一部である包括的な PDF ライブラリです。強力ではありますが、バンドルされたライセンスモデルと座標ベースの API は制限的になることがあります。このガイドでは、Syncfusion PDF API の全表面領域をカバーする IronPDF への徹底的な移行パスを提供します。

## 目次

1. [Syncfusion から移行する理由](#Syncfusion-から移行する理由)
2. [インストールとライセンス](#インストールとライセンス)
3. [完全な API マッピング](#完全な-API-マッピング)
4. [ドキュメントの作成と管理](#ドキュメントの作成と管理)
5. [テキストとグラフィックスの操作](#テキストとグラフィックスの操作)
6. [テーブルとデータグリッド](#テーブルとデータグリッド)
7. [既存の PDF との作業](#既存の-PDF-との作業)
8. [ページ操作](#ページ操作)
9. [ヘッダー、フッター、ページ番号](#ヘッダー、フッター、ページ番号)
10. [セキュリティと暗号化](#セキュリティと暗号化)
11. [フォームと注釈](#フォームと注釈)
12. [テキスト抽出](#テキスト抽出)
13. [マージと分割](#マージと分割)
14. [ウォーターマークとスタンプ](#ウォーターマークとスタンプ)
15. [ブックマークとナビゲーション](#ブックマークとナビゲーション)
16. [添付ファイル](#添付ファイル)
17. [デジタル署名](#デジタル署名)
18. [圧縮と最適化](#圧縮と最適化)
19. [移行チェックリスト](#移行チェックリスト)

---

## Syncfusion から移行する理由

### バンドルライセンスの問題

Syncfusion のライセンスモデルは大きな課題を生み出します：

1. **スイートのみの購入**：PDF ライブラリを単独で購入することはできず、Essential Studio 全体を購入する必要があります
2. **コミュニティライセンスの制限**：無料層は、年間売上 < $1M かつ開発者 < 5 名が必要です
3. **複雑なデプロイメントライセンス**：Web、デスクトップ、サーバーデプロイメント用の異なるライセンスがあります
4. **年間更新が必要**：年次コストのあるサブスクリプションモデルです
5. **開発者ごとの価格設定**：チームサイズに比例してコストが増加します
6. **スイートの膨張**：必要ないかもしれない 1000+ のコンポーネントが含まれています

### IronPDF の利点

| 項目 | Syncfusion PDF | IronPDF |
|------|----------------|---------|
| 購入モデル | スイートバンドルのみ | スタンドアロン |
| ライセンス | 複雑な階層 | シンプルな開発者ごと |
| コミュニティ制限 | <$1M かつ <5 devs | 無料トライアル、その後ライセンス |
| デプロイメント | 複数のライセンスタイプ | 1つのライセンスで全てをカバー |
| API スタイル | 座標ベースのグラフィックス | HTML/CSS ファースト |
| HTML サポート | BlinkBinaries が必要 | ネイティブ Chromium |
| CSS サポート | 限定的 | 完全な CSS3/flexbox/grid |
| 依存関係 | 複数のパッケージ | 単一の NuGet |

---

## インストールとライセンス

### Syncfusion パッケージの削除

```bash
# Syncfusion PDF パッケージを全て削除
dotnet remove package Syncfusion.Pdf.Net.Core
dotnet remove package Syncfusion.HtmlToPdfConverter.Net.Windows
dotnet remove package Syncfusion.Pdf.Imaging.Net.Core
dotnet remove package Syncfusion.Pdf.OCR.Net.Core

# ライセンス登録を削除
# 削除：Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("YOUR_KEY");
```

### IronPDF のインストール

```bash
dotnet add package IronPdf
```

### ライセンス設定

**Syncfusion:**
```csharp
// Syncfusion の呼び出し前に登録が必須
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("YOUR-SYNCFUSION-KEY");
```

**IronPDF:**
```csharp
// 起動時に一度
IronPdf.License.LicenseKey = "YOUR-IRONPDF-KEY";
```

---

## 完全な API マッピング

### コアドキュメントクラス

| Syncfusion | IronPDF | メモ |
|------------|---------|------|
| `PdfDocument` | `ChromePdfRenderer` / `PdfDocument` | PDF の作成または読み込み |
| `PdfPage` | N/A (HTML がページを自動生成) | HTML から自動的にページ |
| `PdfLoadedDocument` | `PdfDocument.FromFile()` | 既存の PDF を読み込み |
| `PdfLoadedPage` | `pdf.Pages[index]` | ページへのアクセス |
| `PdfDocumentBase` | `PdfDocument` | ベースドキュメント |
| `PdfPageBase` | N/A | HTML/CSS を使用 |

### グラフィックスと描画

| Syncfusion PdfGraphics | IronPDF | メモ |
|-----------------------|---------|------|
| `graphics.DrawString()` | HTML テキスト要素 | `<p>`, `<h1>`, `<span>` |
| `graphics.DrawLine()` | CSS ボーダーまたは `<hr>` | HTML/CSS |
| `graphics.DrawRectangle()` | `<div>` に CSS を使用 | CSS ボーダー |
| `graphics.DrawEllipse()` | CSS `border-radius` | 円は 50% |
| `graphics.DrawImage()` | `<img>` タグ | HTML 画像 |
| `graphics.DrawPath()` | SVG `<path>` | SVG グラフィックス |
| `graphics.DrawArc()` | SVG アーク | SVG |
| `graphics.DrawBezier()` | SVG ベジェ | SVG |
| `graphics.DrawPolygon()` | SVG `<polygon>` | SVG |
| `graphics.DrawPie()` | SVG または CSS | SVG |
| `graphics.SetClip()` | CSS `clip-path` | CSS クリッピング |
| `graphics.RotateTransform()` | CSS `transform: rotate()` | CSS 変換 |
| `graphics.TranslateTransform()` | CSS `transform: translate()` | CSS 変換 |
| `graphics.ScaleTransform()` | CSS `transform: scale()` | CSS 変換 |
| `graphics.Save()` / `Restore()` | N/A | CSS が状態を管理 |

### フォントとテキスト

| Syncfusion | IronPDF | メモ |
|------------|---------|------|
| `PdfStandardFont` | CSS `font-family` | システムフォント |
| `PdfTrueTypeFont` | CSS `@font-face` | カスタムフォント |
| `PdfCjkStandardFont` | CJK フォントを使用した CSS | アジアフォント |
| `PdfFontFamily.Helvetica` | `font-family: Helvetica` | CSS |
| `PdfFontFamily.TimesRoman` | `font-family: 'Times New Roman'` | CSS |
| `PdfFontFamily.Courier` | `font-family: 'Courier New'` | CSS |
| `PdfFontStyle.Bold` | `font-weight: bold` | CSS |
| `PdfFontStyle.Italic` | `font-style: italic` | CSS |
| `PdfFontStyle.Underline` | `text-decoration: underline` | CSS |
| `PdfFontStyle.Strikeout` | `text-decoration: line-through` | CSS |
| `font.MeasureString()` | N/A (自動) | CSS が処理 |
| `PdfTextElement` | HTML テキスト要素 | `<p>`, `<div>` |
| `PdfTextWebLink` | `<a href="">` | HTML リンク |

### 色とブラシ

| Syncfusion | IronPDF | メモ |
|------------|---------|------|
| `PdfBrushes.Black` | `color: black` | CSS 色 |
| `PdfBrushes.Red` | `color: red` | CSS 色 |
| `PdfSolidBrush` | CSS `color` / `background-color` | CSS |
| `PdfLinearGradientBrush` | CSS `linear-gradient()` | CSS グラデーション |
| `PdfRadialGradientBrush` | CSS `radial-gradient()` | CSS グラデーション |
| `PdfColor` | CSS 色値 | hex, rgb, rgba |
| `PdfPen` | CSS `border` | CSS ボーダー |
| `PdfPens.Black` | `border-color: black` | CSS |

### ページ設定

| Syncfusion | IronPDF | メモ |
|------------|---------|------|
| `document.PageSettings.Size` | `RenderingOptions.PaperSize` | 用紙サイズ |
| `PdfPageSize.A4` | `PdfPaperSize.A4` | A4 用紙 |
| `PdfPageSize.Letter` | `PdfPaperSize.Letter` | レター用紙 |
| `document.PageSettings.Orientation` | `RenderingOptions.PaperOrientation` | 方向 |
| `PdfPageOrientation.Landscape` | `PdfPaperOrientation.Landscape` | 横向き |
| `document.PageSettings.Margins` | `RenderingOptions.MarginTop/Bottom/Left/Right` | 余白 |
| `page.Size.Width` | `RenderingOptions.PaperSize` | ページ幅 |
| `page.Size.Height` | `RenderingOptions.PaperSize` | ページ高さ |

### テーブル (PdfGrid)

| Syncfusion PdfGrid | IronPDF | メモ |
|-------------------|---------|------|
| `new PdfGrid()` | HTML `<table>` | HTML テーブル |
| `grid.DataSource = data` | データから HTML を構築 | テンプレート化 |
| `grid.Columns.Add()` | `<th>` 要素 | テーブルヘッダー |
| `grid.Rows.Add()` | `<tr>` 要素 | テーブル行 |
| `PdfGridCell` | `<td>` 要素 | テーブルセル |
| `grid.Headers` | `<thead>` | テーブルヘッダー |
| `PdfGridCellStyle` | CSS スタイル | セルスタイリング |
| `grid.Draw(page, point)` | HTML では自動 | 手動位置指定なし |
| `PdfGridLayoutResult` | N/A | 自動レイアウト |
| `PdfLightTable` | HTML `<table>` | よりシンプルなテーブル |

### HTML 変換

| Syncfusion | IronPDF | メモ |
|------------|---------|------|
| `HtmlToPdfConverter` | `ChromePdfRenderer` | メインコンバーター |
| `converter.Convert(url)` | `renderer.RenderUrlAsPdf(url)` | URL から PDF へ |
| `converter.Convert(html, baseUrl)` | `renderer.RenderHtmlAsPdf(html)` | HTML から PDF へ |
| `BlinkConverterSettings` | `ChromePdfRenderOptions` | 設定 |
| `settings.ViewPortSize` | `RenderingOptions.ViewPortWidth` | ビューポート |
| `settings.PdfPageSize` | `RenderingOptions.PaperSize` | 用紙サイズ |
| `settings.Margin` | `RenderingOptions.Margin*` | 余白 |
| `settings.EnableJavaScript` | `RenderingOptions.EnableJavaScript` | JS 実行 |

### セキュリティと暗号化

| Syncfusion | IronPDF | メモ |
|------------|---------|------|
| `document.Security.UserPassword` | `pdf.SecuritySettings.UserPassword` | ユーザーパスワード |
| `document.Security.OwnerPassword` | `pdf.SecuritySettings.OwnerPassword` | オーナーパスワード |
| `document.Security.Permissions` | `pdf.SecuritySettings.Allow*` | 権限 |
| `PdfPermissionsFlags.Print` | `AllowUserPrinting` | 印刷権限 |
| `PdfPermissionsFlags.CopyContent` | `AllowUserCopyPasteContent` | コピー権限 |
| `PdfPermissionsFlags.EditContent` | `AllowUserEdits` | 編集権限 |
| `PdfPermissionsFlags.EditAnnotations` | `AllowUserAnnotations` | 注釈権限 |
| `PdfPermissionsFlags.FillFields` | `AllowUserFormData` | フォーム権限 |
| `PdfEncryptionAlgorithm.AES` | 256ビット暗号化 | 暗号化 |
| `document.Security.KeySize` | 自動 | キーサイズ |

### メタデータ

| Syncfusion | IronPDF | メモ |
|------------|---------|------|
| `document.DocumentInformation.Title` | `pdf.MetaData.Title` | タイトル |
| `document.DocumentInformation.Author` | `pdf.MetaData.Author` | 著者 |
| `document.DocumentInformation.Subject` | `pdf.MetaData.Subject` | 主題 |
| `document.DocumentInformation.Keywords` | `pdf.MetaData.Keywords` | キーワード |
| `document.DocumentInformation.Creator` | `pdf.MetaData.Creator` | 作成者 |
| `document.DocumentInformation.Producer` | `pdf.MetaData.Producer` | 製作者 |
| `document.DocumentInformation.CreationDate` | `pdf.MetaData.CreationDate` | 日付 |

### ブックマーク

| Syncfusion | IronPDF | メモ |
|------------|---------|------|
| `document.Bookmarks.Add()` | `pdf.BookMarks.AddBookMarkAtStart()` | ブックマークの追加 |
| `PdfBookmark` | `PdfBookmark` | ブックマークオブジェクト |
| `bookmark.Title` | `bookmark.Text` | ブックマークテキスト |
| `bookmark.Destination` | `bookmark.PageIndex` | 対象ページ |
| `bookmark.TextStyle` | N/A | スタイリング |
| `bookmark.Color` | N/A | 色 |

### 注釈

| Syncfusion | IronPDF | メモ |
|------------|---------|------|
| `PdfTextMarkupAnnotation` | `pdf.ApplyStamp()` | マークアップ |
| `PdfPopupAnnotation` | N/A | ポップアップノート |
| `PdfLineAnnotation` | PDF 編集を使用 | 線 |
| `PdfRubberStampAnnotation` | `pdf.ApplyStamp()` | スタンプ |
| `PdfFreeTextAnnotation` | `TextStamper` | フリーテキスト |
| `PdfInkAnnotation` | N/A | インク |
| `page.Annotations.Add()` | `pdf.ApplyStamp()` | 注釈の追加 |

### フォーム

| Syncfusion | IronPDF | メモ |
|------------|---------|------|
| `PdfForm` | `pdf.Form` | フォームオブジェクト |
| `PdfTextBoxField` | `pdf.Form.FindFormField()` | テキスト