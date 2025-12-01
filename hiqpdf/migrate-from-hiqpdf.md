---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [hiqpdf/migrate-from-hiqpdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/hiqpdf/migrate-from-hiqpdf.md)
🇯🇵 **日本語:** [hiqpdf/migrate-from-hiqpdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/hiqpdf/migrate-from-hiqpdf.md)

---

# 移行ガイド: HiQPdf → IronPDF

## 目次

1. [HiQPdfからIronPDFへの移行理由](#hiqpdfからironpdfへの移行理由)
2. [開始前に](#開始前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンス比較](#パフォーマンス比較)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## HiQPdfからIronPDFへの移行理由

### HiQPdfの制限

HiQPdfは商用のHTMLからPDFへのライブラリで、いくつかの懸念点があります：

1. **制限的な「無料」バージョン**：無料版は3ページ制限があり、侵入的なウォーターマークが付いています。実質的に本番環境で使用不可
2. **古いWebKitエンジン**：古いWebKitベースのレンダリングエンジンを使用しており、現代のJavaScriptフレームワークに対応していません
3. **.NET Coreサポートが不明確**：ドキュメントでは.NET Core / .NET 5+のサポートを明確にしておらず、別のNuGetパッケージが必要です
4. **断片化されたパッケージ**：異なるプラットフォーム用の複数のNuGetパッケージ（HiQPdf、HiQPdf.NetCore、HiQPdf.Client）
5. **複雑なAPI**：`Document`、`Header`、`Footer`プロパティチェーンを通じて冗長な設定が必要です
6. **限定的なJavaScriptサポート**：WebKitエンジンはReact、Angular、Vueなどの現代のJSフレームワークで問題があります

### 代わりにIronPDFが提供するもの

| 項目 | HiQPdf | IronPDF |
|--------|--------|---------|
| レンダリングエンジン | WebKitベース（古い） | 現代のChromium |
| 無料層 | 3ページ制限 + ウォーターマーク | 30日間の完全なトライアル |
| 現代のJSサポート | 限定的 | 完全（React、Angular、Vue） |
| .NET Core/5+サポート | 複数のパッケージが必要 | 単一の統合パッケージ |
| APIデザイン | 複雑なプロパティチェーン | クリーンなフルエントAPI |
| CSS3サポート | 部分的 | 完全サポート |
| ドキュメント | 断片化 | 包括的 |
| NuGetパッケージ | 複数のバリアント | 単一パッケージ |

---

## 開始前に

### 前提条件

1. **.NETバージョン**：IronPDFは.NET Framework 4.6.2+および.NET Core 3.1+ / .NET 5+をサポートします
2. **ライセンスキー**：[IronPDFウェブサイト](https://ironpdf.com/licensing/)から入手してください
3. **HiQPdfの削除**：すべてのHiQPdf NuGetパッケージバリアントを削除する予定です

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

**更新前（HiQPdf）：**
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

**更新後（IronPDF）：**
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

**更新前（HiQPdf）：**
```csharp
// コンストラクタまたはプロパティでライセンス
HtmlToPdf converter = new HtmlToPdf();
converter.SerialNumber = "HIQPDF-SERIAL-NUMBER";
```

**更新後（IronPDF）：**
```csharp
// アプリケーション起動時（Program.cs / Startup.cs）にグローバルに設定
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

// またはappsettings.jsonで
// { "IronPdf": { "LicenseKey": "YOUR-KEY" } }
```

---