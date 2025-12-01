---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [nrecopdfgenerator/migrate-from-nrecopdfgenerator.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/nrecopdfgenerator/migrate-from-nrecopdfgenerator.md)
🇯🇵 **日本語:** [nrecopdfgenerator/migrate-from-nrecopdfgenerator.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/nrecopdfgenerator/migrate-from-nrecopdfgenerator.md)

---

# NReco.PdfGeneratorからIronPDFへの移行方法は？

## なぜ移行するのか？

NReco.PdfGeneratorは、非推奨のwkhtmltopdfバイナリをラップしており、そのすべてのセキュリティ脆弱性とレンダリングの制限を継承しています。このガイドでは、IronPDFの現代的で安全なChromiumベースのエンジンへの完全な移行パスを提供します。

### NReco.PdfGeneratorの重大な問題点

1. **セキュリティ脆弱性**: wkhtmltopdfのCVE（20以上の文書化された脆弱性）をすべて継承
   - CVE-2020-21365: サーバーサイドリクエストフォージェリ（SSRF）
   - CVE-2022-35583: HTMLインジェクションを介したローカルファイル読み取り
   - CVE-2022-35580: リモートコード実行の可能性
   - パッチが利用できない—wkhtmltopdfは2020年以降放棄されている

2. **ウォーターマーク付きの無料バージョン**: 本番環境での使用には不透明な価格設定の有料ライセンスが必要

3. **非推奨のレンダリングエンジン**: WebKit Qt（2012年頃）で、CSS3/JSのサポートが限定的：
   - CSS GridやFlexboxがない
   - 現代のJavaScript（ES6+）がない
   - Webフォントのサポートが悪い
   - CSS変数やカスタムプロパティがない

4. **外部バイナリ依存**: プラットフォームごとにwkhtmltopdfバイナリを管理する必要がある

5. **アクティブな開発がない**: 基盤となるエンジンの更新なしにラッパーのメンテナンス

6. **限定的な非同期サポート**: Webアプリケーションでスレッドをブロックする同期API

### IronPDFの利点

| 項目 | NReco.PdfGenerator | IronPDF |
|------|-------------------|---------|
| レンダリングエンジン | WebKit Qt (2012) | Chromium (現在) |
| セキュリティ | 20以上のCVE、パッチなし | アクティブなセキュリティ更新 |
| CSSサポート | CSS2.1、限定的なCSS3 | 完全なCSS3、Grid、Flexbox |
| JavaScript | 基本的なES5 | 完全なES6+、async/await |
| 依存関係 | 外部のwkhtmltopdfバイナリ | 自己完結型 |
| 非同期サポート | 同期のみ | 完全なasync/await |
| Webフォント | 限定的 | 完全なGoogleフォント、@font-face |
| ライセンス | 不透明な価格設定、営業に問い合わせ | 透明な価格設定 |
| 無料トライアル | ウォーターマークあり | 完全な機能 |

---

## NuGetパッケージの変更

```bash
# NReco.PdfGeneratorを削除
dotnet remove package NReco.PdfGenerator

# IronPDFをインストール
dotnet add package IronPdf
```

**また、デプロイメントからwkhtmltopdfバイナリを削除してください：**
- プロジェクトから`wkhtmltopdf.exe`、`wkhtmltox.dll`を削除
- wkhtmltopdfのインストールスクリプトを削除
- プラットフォーム固有のバイナリフォルダを削除

---

## 名前空間の変更

| NReco.PdfGenerator | IronPDF |
|-------------------|---------|
| `using NReco.PdfGenerator;` | `using IronPdf;` |
| | `using IronPdf.Rendering;` (列挙型用) |

---

## 完全なAPIマッピング

### コアクラス

| NReco.PdfGenerator | IronPDF | 備考 |
|-------------------|---------|------|
| `HtmlToPdfConverter` | `ChromePdfRenderer` | メインレンダラー |
| `PageMargins` | 個別のマージンプロパティ | MarginTop, MarginBottom, など |
| `PageOrientation` | `PdfPaperOrientation` | 列挙型 |
| `PageSize` | `PdfPaperSize` | 列挙型 |

### レンダリングメソッド

| NReco.PdfGenerator | IronPDF | 備考 |
|-------------------|---------|------|
| `GeneratePdf(html)` | `RenderHtmlAsPdf(html)` | PdfDocumentを返す |
| `GeneratePdf(html, coverHtml)` | `RenderHtmlAsPdf()` + `Merge()` | 複数ステップ |
| `GeneratePdfFromFile(url, output)` | `RenderUrlAsPdf(url)` | 直接URLサポート |
| `GeneratePdfFromFile(htmlPath, output)` | `RenderHtmlFileAsPdf(path)` | ファイルパス |
| _(非同期はサポートされていない)_ | `RenderHtmlAsPdfAsync(html)` | 非同期バージョン |
| _(非同期はサポートされていない)_ | `RenderUrlAsPdfAsync(url)` | 非同期バージョン |

### ページ設定

| NReco.PdfGenerator | IronPDF | 備考 |
|-------------------|---------|------|
| `PageWidth = 210` | `RenderingOptions.PaperSize = PdfPaperSize.A4` | 列挙型を使用 |
| `PageHeight = 297` | `RenderingOptions.SetCustomPaperSizeinMilimeters(w, h)` | カスタムサイズ |
| `Orientation = PageOrientation.Landscape` | `RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape` | ランドスケープ |
| `Size = PageSize.A4` | `RenderingOptions.PaperSize = PdfPaperSize.A4` | 用紙サイズの列挙型 |

### マージン

| NReco.PdfGenerator | IronPDF | 備考 |
|-------------------|---------|------|
| `Margins.Top = 10` | `RenderingOptions.MarginTop = 10` | ミリメートル単位 |
| `Margins.Bottom = 10` | `RenderingOptions.MarginBottom = 10` | ミリメートル単位 |
| `Margins.Left = 10` | `RenderingOptions.MarginLeft = 10` | ミリメートル単位 |
| `Margins.Right = 10` | `RenderingOptions.MarginRight = 10` | ミリメートル単位 |
| `new PageMargins { ... }` | 個別のプロパティ | マージンオブジェクトなし |

### レンダリングオプション

| NReco.PdfGenerator | IronPDF | 備考 |
|-------------------|---------|------|
| `Zoom = 1.5f` | `RenderingOptions.Zoom = 150` | パーセンテージ（100 = 100%） |
| `Quiet = true` | _(デフォルトの挙動)_ | IronPDFはデフォルトで静か |
| `LogReceived += handler` | `Logger.EnableDebugging = true` | デバッグログ |
| `CustomWkHtmlArgs = "--disable-smart-shrinking"` | `RenderingOptions.FitToPaperMode` | 組み込みオプション |
| `CustomWkHtmlPageArgs = "--no-background"` | `RenderingOptions.PrintHtmlBackgrounds = false` | ネイティブプロパティ |

### ヘッダーとフッター

| NReco.PdfGenerator | IronPDF | 備考 |
|-------------------|---------|------|
| `PageHeaderHtml = "<div>..."` | `RenderingOptions.HtmlHeader` | HtmlHeaderFooterオブジェクト |
| `PageFooterHtml = "<div>..."` | `RenderingOptions.HtmlFooter` | HtmlHeaderFooterオブジェクト |
| `HeaderSpacing = 10` | `HtmlHeader.MaxHeight = 25` | 高さ（ミリメートル） |
| `FooterSpacing = 10` | `HtmlFooter.MaxHeight = 25` | 高さ（ミリメートル） |

### プレースホルダ変数

| NReco.PdfGenerator (wkhtmltopdf) | IronPDF | 備考 |
|--------------------------------|---------|------|
| `[page]` | `{page}` | 現在のページ番号 |
| `[topage]` | `{total-pages}` | 総ページ数 |
| `[date]` | `{date}` | 現在の日付 |
| `[time]` | `{time}` | 現在の時間 |
| `[title]` | `{html-title}` | 文書のタイトル |
| `[webpage]` | `{url}` | ソースURL |

### 出力処理

| NReco.PdfGenerator | IronPDF | 備考 |
|-------------------|---------|------|
| `byte[] pdfBytes = GeneratePdf(html)` | `PdfDocument pdf = RenderHtmlAsPdf(html)` | オブジェクトを返す |
| `File.WriteAllBytes(path, bytes)` | `pdf.SaveAs(path)` | 直接保存 |
| `return pdfBytes` | `return pdf.BinaryData` | バイト配列を取得 |
| `new MemoryStream(pdfBytes)` | `pdf.Stream` | ストリームを取得 |

---