---
**  (Japanese Translation)**

 **English:** [hiqpdf/migrate-from-hiqpdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/hiqpdf/migrate-from-hiqpdf.md)
 **:** [hiqpdf/migrate-from-hiqpdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/hiqpdf/migrate-from-hiqpdf.md)

---
# HiQPdfからIronPDFへの移行方法は？

## 目次

1. [HiQPdfからIronPDFへ移行する理由](#hiqpdfからironpdfへ移行する理由)
2. [開始する前に](#開始する前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンス比較](#パフォーマンス比較)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## HiQPdfからIronPDFへ移行する理由

### HiQPdfの制限

HiQPdfは商用のHTMLからPDFへのライブラリで、いくつかの問題点があります：

1. **制限された「無料」バージョン**：無料版は3ページ制限があり、侵入的なウォーターマークが付いています。実質的に本番環境で使用不可能
2. **古いWebKitエンジン**：古いWebKitベースのレンダリングエンジンを使用しており、現代のJavaScriptフレームワークに対応していません
3. **.NET Coreサポートが不明確**：ドキュメントでは.NET Core / .NET 5+のサポートについて明確に説明されておらず、別のNuGetパッケージが必要です
4. **パッケージが分断**：異なるプラットフォーム用の複数のNuGetパッケージ（HiQPdf、HiQPdf.NetCore、HiQPdf.Client）
5. **複雑なAPI**：`Document`、`Header`、`Footer`プロパティチェーンを通じて冗長な設定が必要です
6. **限定的なJavaScriptサポート**：WebKitエンジンはReact、Angular、Vueなどの現代のJSフレームワークに対応していません

### 代わりにIronPDFが提供するもの

| 項目 | HiQPdf | IronPDF |
|--------|--------|---------|
| レンダリングエンジン | WebKitベース（古い） | 最新のChromium |
| 無料版 | 3ページ制限 + ウォーターマーク | 30日間の完全トライアル |
| 現代のJSサポート | 限定的 | 完全（React、Angular、Vue） |
| .NET Core/5+サポート | 複数のパッケージが必要 | 単一の統合パッケージ |
| APIデザイン | 複雑なプロパティチェーン | クリーンなフルエントAPI |
| CSS3サポート | 部分的 | 完全サポート |
| ドキュメント | 分断された | 包括的 |
| NuGetパッケージ | 複数のバリアント | 単一のパッケージ |

---

## 開始する前に

### 前提条件

1. **.NETバージョン**：IronPDFは.NET Framework 4.6.2+および.NET Core 3.1+ / .NET 5+をサポートしています
2. **ライセンスキー**：[IronPDFウェブサイト](https://ironpdf.com/licensing/)から取得してください
3. **HiQPdfの削除**：すべてのHiQPdf NuGetパッケージバリアントを削除する計画を立ててください

### HiQPdfの使用箇所の特定

コードベース内のすべてのHiQPdfの使用箇所を見つけます：

```bash
# HiQPdf名前空間の使用を検索
grep -r "using HiQPdf\|HtmlToPdf\|PdfDocument" --include="*.cs" .

# HiQPdfドキュメント設定を検索
grep -r "BrowserWidth\|TriggerMode\|PageOrientation\|ConvertHtmlToMemory" --include="*.cs" .

# ヘッダー/フッターの使用を検索
grep -r "\.Header\.\|\.Footer\.\|HtmlToPdfVariableElement" --include="*.cs" .

# NuGet参照を検索
grep -r "HiQPdf" --include="*.csproj" .
```

### 依存関係の監査

HiQPdfパッケージバリアントをチェックします：

```bash
grep -r "HiQPdf\|hiqpdf" --include="*.csproj" .
```

一般的なパッケージ名：
- `HiQPdf`
- `HiQPdf.Free`
- `HiQPdf.NetCore`
- `HiQPdf.NetCore.x64`
- `HiQPdf.Client`

---

## クイックスタート移行

### ステップ1：IronPDFのインストール

```bash
# すべてのHiQPdfバリアントを削除
dotnet remove package HiQPdf
dotnet remove package HiQPdf.Free
dotnet remove package HiQPdf.NetCore
dotnet remove package HiQPdf.NetCore.x64
dotnet remove package HiQPdf.Client

# IronPDFをインストール（すべてのプラットフォーム用の単一パッケージ）
dotnet add package IronPdf
```

### ステップ2：コードの更新

**変更前（HiQPdf）：**
```csharp
using HiQPdf;

public class PdfService
{
    public byte[] GeneratePdf(string html)
    {
        HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

        // ブラウザ設定
        htmlToPdfConverter.BrowserWidth = 1024;

        // ページ設定
        htmlToPdfConverter.Document.PageSize = PdfPageSize.A4;
        htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;
        htmlToPdfConverter.Document.Margins.Left = 10;
        htmlToPdfConverter.Document.Margins.Right = 10;

        // メモリへの変換
        byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(html, null);
        return pdfBuffer;
    }
}
```

**変更後（IronPDF）：**
```csharp
using IronPdf;

public class PdfService
{
    public byte[] GeneratePdf(string html)
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var renderer = new ChromePdfRenderer();

        // ビューポート設定
        renderer.RenderingOptions.ViewPortWidth = 1024;

        // ページ設定
        renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;
        renderer.RenderingOptions.MarginLeft = 10;
        renderer.RenderingOptions.MarginRight = 10;

        // 変換して返す
        var pdf = renderer.RenderHtmlAsPdf(html);
        return pdf.BinaryData;
    }
}
```

### ステップ3：ライセンス設定の更新

**変更前（HiQPdf）：**
```csharp
// コンストラクタまたはプロパティでのライセンス
HtmlToPdf converter = new HtmlToPdf();
converter.SerialNumber = "HIQPDF-SERIAL-NUMBER";
```

**変更後（IronPDF）：**
```csharp
// アプリケーション起動時にグローバルに設定（Program.cs / Startup.cs）
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

// またはappsettings.jsonで
// { "IronPdf": { "LicenseKey": "YOUR-KEY" } }
```

---

## 完全なAPIリファレンス

### 主要クラスのマッピング

| HiQPdfクラス | IronPDFクラス | 備考 |
|-------------|--------------|-------|
| `HtmlToPdf` | `ChromePdfRenderer` | 主要な変換クラス |
| `PdfDocument` | `PdfDocument` | 同じ名前、異なる名前空間 |
| `PdfPage` | `pdf.Pages[i]` | インデクサー経由でアクセス |
| `PdfDocumentControl` | `RenderingOptions` | 設定 |
| `PdfHeader` / `PdfDocumentHeader` | `HtmlHeaderFooter` | ヘッダー設定 |
| `PdfFooter` / `PdfDocumentFooter` | `HtmlHeaderFooter` | フッター設定 |
| `HtmlToPdfVariableElement` | `HtmlHeaderFooter.HtmlFragment` | ヘッダー/フッター内のHTML |

### 変換メソッドのマッピング

| HiQPdfメソッド | IronPDFメソッド | 備考 |
|--------------|----------------|-------|
| `ConvertHtmlToMemory(html, baseUrl)` | `RenderHtmlAsPdf(html, baseUrl)` | `PdfDocument`を返す |
| `ConvertUrlToMemory(url)` | `RenderUrlAsPdf(url)` | `PdfDocument`を返す |
| `ConvertHtmlToFile(html, baseUrl, path)` | `RenderHtmlAsPdf(html).SaveAs(path)` | メソッドをチェーン |
| `ConvertUrlToFile(url, path)` | `RenderUrlAsPdf(url).SaveAs(path)` | メソッドをチェーン |
| `ConvertHtmlToPdfDocument(html, baseUrl)` | `RenderHtmlAsPdf(html)` | `PdfDocument`を返す |

### HtmlToPdfプロパティのマッピング

| HiQPdfプロパティ | IronPDFプロパティ | 備考 |
|----------------|------------------|-------|
| `BrowserWidth` | `RenderingOptions.ViewPortWidth` | ピクセル単位 |
| `BrowserHeight` | `RenderingOptions.ViewPortHeight` | ピクセル単位 |
| `TriggerMode` | `RenderingOptions.WaitFor` | 待機条件 |
| `WaitBeforeConvert` | `RenderingOptions.RenderDelay` | ミリ秒単位 |
| `TrimToBrowserWidth` | 該当なし | IronPDFが自動的に処理 |
| `Document.PageSize` | `RenderingOptions.PaperSize` | 列挙型を使用 |
| `Document.PageOrientation` | `RenderingOptions.PaperOrientation` | `Portrait`/`Landscape` |
| `Document.Margins.Top` | `RenderingOptions.MarginTop` | mm単位（ポイントではない！） |
| `Document.Margins.Bottom` | `RenderingOptions.MarginBottom` | mm |
| `Document.Margins.Left` | `RenderingOptions.MarginLeft` | mm |
| `Document.Margins.Right` | `RenderingOptions.MarginRight` | mm |
| `Document.FitPageWidth` | `RenderingOptions.FitToPaperMode` | スケーリングオプション |
| `Document.FitPageHeight` | `RenderingOptions.FitToPaperMode` | スケーリングオプション |
| `Document.ResizePageWidth` | 該当なし | IronPDFが自動的に処理 |
| `SerialNumber` | `IronPdf.License.LicenseKey` | グローバルに設定 |

### TriggerModeのマッピング

| HiQPdf TriggerMode | IronPDF相当 | 備考 |
|-------------------|-------------------|-------|
| `TriggerMode.Auto` | デフォルト動作 | 特別な設定なし |
| `TriggerMode.WaitTime` | `RenderingOptions.RenderDelay = ms` | 固定時間待機 |
| `TriggerMode.Manual` | `RenderingOptions.WaitFor.JavaScript(expr)` | JS条件待機 |
| `WaitBeforeConvert` | `RenderingOptions.RenderDelay` | ミリ秒 |

### ヘッダー/フッターマッピング

| HiQPdfプロパティ | IronPDFプロパティ | 備考 |
|----------------|------------------|-------|
| `Document.Header.Enabled` | ヘッダーを無効にする場合は`null`に設定 | 設定しない場合は無効 |
| `Document.Header.Height` | `HtmlHeaderFooter` CSSの高さ | HTML/CSSで |
| `Document.Header.HtmlSource` | `HtmlHeaderFooter.HtmlFragment` | HTML文字列 |
| `Document.Footer.Enabled` | フッターを無効にする場合は`null`に設定 | 設定しない場合は無効 |
| `Document.Footer.Height` | `HtmlHeaderFooter` CSSの高さ | HTML/CSSで |
| `Document.Footer.HtmlSource` | `HtmlHeaderFooter.HtmlFragment` | HTML文字列 |
| `Document.Footer.DisplayOnFirstPage` | `HtmlHeaderFooter.FirstPageNumber` | ページをスキップ |
| `{CrtPage}`プレースホルダー | `{page}` | 現在のページ |
| `{PageCount}`プレースホルダー | `{total-pages}` | 合計ページ数 |
| `{DocTitle}`プレースホルダー | `{title}` | ドキュメントタイトル |
| `{DocSubject}`プレースホルダー | 該当なし | カスタムHTMLを使用 |
| `{DocAuthor}`プレースホルダー | 該当なし | カスタムHTMLを使用 |
| `{CrtPageURL}`プレースホルダー | `{url}` | 現在のURL |
| `{Date}`プレースホルダー | `{date}` | 現在の日付 |
| `{Time}`プレースホルダー | `{time}` | 現在の時刻 |

### PdfDocumentメソッドのマッピング

| HiQPdfメソッド | IronPDFメソッド | 備考 |
|--------------|----------------|-------|
| `PdfDocument.FromFile(path)` | `PdfDocument.FromFile(path)` | 同じ |
| `document.AddDocument(other)` | `PdfDocument.Merge(doc1, doc2)` | 静的メソッド |
| `document.Save(path)` | `pdf.SaveAs(path)` | 異なる名前 |
| `document.WriteToFile(path)` | `pdf.SaveAs(path)` | 同じ結果 |
| `document.WriteToMemory()` | `pdf.BinaryData` | プロパティ |
| `document.Pages.Count` | `pdf.PageCount` | プロパティ |
| `document.Pages[i]` | `pdf.Pages[i]` | 同じ構文 |
| `document.RemovePage(index)` | `pdf.RemovePage(index)` | 同じ |
| `document.InsertPage(index, page)` | `pdf.InsertPdf(index, other)` | わずかに異なる |
| `page.Rotate(degrees)` | `pdf.RotateAllPages(rotation)` | ページごとに |

---