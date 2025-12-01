---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [pdfpig/migrate-from-pdfpig.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/pdfpig/migrate-from-pdfpig.md)
🇯🇵 **日本語:** [pdfpig/migrate-from-pdfpig.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/pdfpig/migrate-from-pdfpig.md)

---

# PdfPigからIronPDFへの移行方法は？

## なぜPdfPigから移行するのか？

PdfPigはPDFからコンテンツを読み取り、抽出するための優れたオープンソースライブラリです。しかし、その機能は既存のドキュメントを解析することに基本的に限定されています。アプリケーションが抽出を超えて成長する必要がある場合—PDFの生成、HTMLの変換、ドキュメントの操作、セキュリティの追加など—PdfPigでは対応できません。

### 読み取り専用の制限

PdfPigはPDF読み取りにのみ焦点を当てています：

1. **PDF生成不可**：HTML、URL、またはプログラムからPDFを作成できません
2. **HTMLからPDFへの変換不可**：WebコンテンツをPDFドキュメントに変換できません
3. **ドキュメント操作不可**：PDFの結合、分割、または変更ができません
4. **セキュリティ機能不可**：パスワード、暗号化、またはデジタル署名を追加できません
5. **透かし/スタンプ不可**：既存のドキュメントに視覚的なオーバーレイを追加できません
6. **フォーム入力不可**：プログラムでPDFフォームを入力できません
7. **基本的なテキストレイアウト**：手動座標位置指定のみで限定的なドキュメント作成

### なぜIronPDFを選ぶのか？

IronPDFは完全なPDFライフサイクル管理を提供します：

- **完全なPDF生成**：HTML、URL、画像などからPDFを作成
- **豊富な抽出**：テキスト、画像、メタデータの抽出（PdfPigのように）
- **ドキュメント操作**：PDFの結合、分割、回転、変更
- **セキュリティ機能**：パスワード、暗号化、デジタル署名
- **透かしとスタンプ**：視覚的なオーバーレイとブランディングを追加
- **フォーム処理**：PDFフォームの入力と平坦化
- **モダンなレンダリング**：完全なCSS3/JSサポートを備えたChromiumベースのエンジン
- **アクティブサポート**：商用サポートと定期的なアップデート

---

## 移行の概要

| 項目 | PdfPig | IronPDF |
|--------|--------|---------|
| 主な焦点 | 読み取り/抽出 | 完全なPDFライフサイクル |
| PDF作成 | 非常に限定的 | 包括的 |
| HTMLからPDFへ | サポートされていません | 完全なChromiumエンジン |
| URLからPDFへ | サポートされていません | 完全サポート |
| テキスト抽出 | 優れている | 優れている |
| 画像抽出 | はい | はい |
| メタデータアクセス | はい | はい |
| PDF操作 | サポートされていません | 結合、分割、回転 |
| 透かし | サポートされていません | 完全サポート |
| セキュリティ/暗号化 | サポートされていません | 完全サポート |
| フォーム入力 | サポートされていません | 完全サポート |
| デジタル署名 | サポートされていません | 完全サポート |
| ライセンス | Apache 2.0（無料） | 商用 |
| サポート | コミュニティ | プロフェッショナル |

---

## NuGetパッケージの変更

```bash
# PdfPigを削除
dotnet remove package PdfPig

# IronPDFをインストール
dotnet add package IronPdf
```

---

## 名前空間の変更

```csharp
// 以前：PdfPig
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Writer;
using UglyToad.PdfPig.DocumentLayoutAnalysis;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

// 以降：IronPDF
using IronPdf;
using IronPdf.Rendering;
```

---

## APIマッピングリファレンス

### ドキュメントの読み込み

| PdfPig | IronPDF | 備考 |
|--------|---------|-------|
| `PdfDocument.Open(path)` | `PdfDocument.FromFile(path)` | ファイルから読み込み |
| `PdfDocument.Open(bytes)` | `PdfDocument.FromBinaryData(bytes)` | バイトから読み込み |
| `PdfDocument.Open(stream)` | `PdfDocument.FromStream(stream)` | ストリームから読み込み |
| `using (var doc = ...)` | `var pdf = ...` | IronPDFはusingを必要としません |

### ページアクセスとプロパティ

| PdfPig | IronPDF | 備考 |
|--------|---------|-------|
| `document.NumberOfPages` | `pdf.PageCount` | 総ページ数 |
| `document.GetPages()` | `pdf.Pages` | ページコレクション |
| `document.GetPage(1)` | `pdf.Pages[0]` | 単一ページ（注：1ベース対0ベース） |
| `page.Width` | `pdf.Pages[i].Width` | ページ幅 |
| `page.Height` | `pdf.Pages[i].Height` | ページ高さ |
| `page.Rotation` | `pdf.Pages[i].Rotation` | ページ回転 |

### テキスト抽出

| PdfPig | IronPDF | 備考 |
|--------|---------|-------|
| `page.Text` | `pdf.Pages[i].Text` | ページテキスト |
| `page.GetWords()` | `pdf.ExtractTextFromPage(i)` | ページからの単語/テキスト |
| (手動ループ) | `pdf.ExtractAllText()` | 一度にすべてのテキスト |
| `page.Letters` | N/A (テキストを使用) | IronPDFはテキストブロックを使用 |
| `word.Text` | N/A (文字列結果) | 直接の文字列アクセス |
| `word.BoundingBox` | N/A | IronPDFは単語の位置を公開しません |

### メタデータ

| PdfPig | IronPDF | 備考 |
|--------|---------|-------|
| `document.Information.Title` | `pdf.MetaData.Title` | ドキュメントのタイトル |
| `document.Information.Author` | `pdf.MetaData.Author` | 著者 |
| `document.Information.Subject` | `pdf.MetaData.Subject` | 件名 |
| `document.Information.Creator` | `pdf.MetaData.Creator` | 作成者 |
| `document.Information.Producer` | `pdf.MetaData.Producer` | 製作者 |
| `document.Information.Keywords` | `pdf.MetaData.Keywords` | キーワード |
| `document.Information.CreationDate` | `pdf.MetaData.CreationDate` | 作成日 |
| `document.Information.ModifiedDate` | `pdf.MetaData.ModifiedDate` | 変更日 |

### PDF作成（主要なパラダイムシフト）

| PdfPig | IronPDF | 備考 |
|--------|---------|-------|
| `new PdfDocumentBuilder()` | `new ChromePdfRenderer()` | 異なるパラダイム |
| `builder.AddPage(PageSize.A4)` | `renderer.RenderHtmlAsPdf(html)` | HTMLベースの作成 |
| `page.AddText(text, size, point, font)` | `<p style='...'>{text}</p>` | CSSベースのスタイリング |
| `builder.Build()` → `byte[]` | `pdf.SaveAs(path)` | 異なる出力 |
| 手動座標位置指定 | HTML/CSSレイアウト | レイアウトアプローチ |

### PdfPigにない機能（IronPDFの新機能）

| 機能 | IronPDFメソッド |
|---------|----------------|
| HTMLからPDFへ | `renderer.RenderHtmlAsPdf(html)` |
| URLからPDFへ | `renderer.RenderUrlAsPdf(url)` |
| PDFの結合 | `PdfDocument.Merge(pdfs)` |
| PDFの分割 | `pdf.CopyPages(start, end)` |
| 透かしの追加 | `pdf.ApplyWatermark(html)` |
| パスワード保護 | `pdf.SecuritySettings.UserPassword` |
| デジタル署名 | `pdf.Sign(certificate)` |
| フォームの入力 | `pdf.Form.GetFieldByName(name).Value` |
| ヘッダー/フッターの追加 | `renderer.RenderingOptions.HtmlHeader` |

---