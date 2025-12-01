---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [dinktopdf/migrate-from-dinktopdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/dinktopdf/migrate-from-dinktopdf.md)
🇯🇵 **日本語:** [dinktopdf/migrate-from-dinktopdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/dinktopdf/migrate-from-dinktopdf.md)

---

# DinkToPdfからIronPDFへの移行方法は？

## 目次
1. [IronPDFへの移行理由](#ironpdfへの移行理由)
2. [開始する前に](#開始する前に)
3. [クイックスタート（5分）](#クイックスタート5分)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティングガイド](#トラブルシューティングガイド)
9. [移行チェックリスト](#移行チェックリスト)
10. [追加リソース](#追加リソース)

---

## IronPDFへの移行理由

### DinkToPdfの重大なセキュリティ問題

DinkToPdfはwkhtmltopdfをラップしており、**修正されていない重大なセキュリティ脆弱性**があります：

1. **CVE-2022-35583 (SSRF)**: 攻撃者が内部ネットワークリソースにアクセスできるサーバーサイドリクエストフォージェリ
2. **プロジェクトの放棄**: wkhtmltopdfは2020年以降メンテナンスされていません
3. **セキュリティパッチの不在**: 既知の脆弱性は修正されることがありません

### DinkToPdfの技術的問題

| 問題 | 影響 |
|-------|--------|
| **スレッドセーフティ** | 本番環境でのSynchronizedConverterのクラッシュ |
| **ネイティブバイナリ** | プラットフォーム固有のバイナリを含む複雑なデプロイメント |
| **CSSの制限** | Flexbox、Grid、または現代的なCSSのサポートなし |
| **JavaScript** | 実行の不一致、タイムアウト |
| **レンダリング** | 古いWebKitエンジン（2015年頃） |
| **メンテナンス** | 最終更新: 2018年 |

### IronPDFの主な利点

| 項目 | DinkToPdf | IronPDF |
|--------|-----------|---------|
| **セキュリティ** | CVE-2022-35583 (SSRF)、未修正 | 既知の脆弱性なし |
| **レンダリングエンジン** | 古いWebKit (2015) | 現代的なChromium |
| **スレッドセーフティ** | 並行使用時のクラッシュ | 完全にスレッドセーフ |
| **ネイティブ依存性** | プラットフォーム固有のバイナリ | 純粋なNuGetパッケージ |
| **CSSサポート** | Flexbox/Gridなし | 完全なCSS3 |
| **JavaScript** | 限定的、不一致 | 完全なサポート |
| **メンテナンス** | 放棄された (2018) | 積極的なメンテナンス |
| **サポート** | コミュニティのみ | プロフェッショナルサポート |

### 機能比較

| 機能 | DinkToPdf | IronPDF |
|---------|-----------|---------|
| HTMLからPDFへ | ✅ (古いエンジン) | ✅ (Chromium) |
| URLからPDFへ | ✅ | ✅ |
| カスタムマージン | ✅ | ✅ |
| ヘッダー/フッター | ✅ (限定的) | ✅ (完全なHTML) |
| CSS3 | ❌ 限定的 | ✅ 完全 |
| Flexbox/Grid | ❌ | ✅ |
| JavaScript | ⚠️ 限定的 | ✅ 完全 |
| PDF操作 | ❌ | ✅ |
| フォーム入力 | ❌ | ✅ |
| デジタル署名 | ❌ | ✅ |
| 暗号化 | ❌ | ✅ |
| ウォーターマーク | ❌ | ✅ |
| マージ/分割 | ❌ | ✅ |

---

## 開始する前に

### 前提条件

- **.NETバージョン**: IronPDFは.NET Framework 4.6.2+、.NET Core 3.1+、.NET 5/6/7/8/9をサポート
- **NuGetアクセス**: nuget.orgからパッケージをインストールできることを確認
- **ライセンスキー**: [IronPDFウェブサイト](https://ironpdf.com/)から取得（無料トライアル利用可能）

### DinkToPdfのすべての参照を見つける

```bash
# コードベース内のすべてのDinkToPdfの使用を検索
grep -r "using DinkToPdf" --include="*.cs" .
grep -r "SynchronizedConverter\|HtmlToPdfDocument\|ObjectSettings" --include="*.cs" .

# NuGetパッケージ参照を検索
grep -r "DinkToPdf" --include="*.csproj" .
grep -r "DinkToPdf" --include="packages.config" .

# wkhtmltopdfバイナリを検索
find . -name "*.dll" | xargs file | grep -i wkhtmltopdf
find . -name "libwkhtmltox*"
```

### 変更点の概要

| 変更 | DinkToPdf | IronPDF | 影響 |
|--------|-----------|---------|--------|
| **コンバーター** | `SynchronizedConverter(new PdfTools())` | `ChromePdfRenderer` | よりシンプルなインスタンス化 |
| **ドキュメント** | `HtmlToPdfDocument` | 直接メソッド呼び出し | ドキュメントオブジェクトなし |
| **設定** | `GlobalSettings` + `ObjectSettings` | `RenderingOptions` | シングルオプションオブジェクト |
| **戻り値の型** | `byte[]` | `PdfDocument` | より強力なオブジェクト |
| **バイナリ** | `libwkhtmltox.dll/so` | なし（管理された） | ネイティブファイルを削除 |
| **スレッドセーフティ** | `SynchronizedConverter`が必要 | デフォルトでスレッドセーフ | よりシンプルなコード |
| **DI** | シングルトンが必要 | 任意のライフタイム | 柔軟性 |

---

## クイックスタート（5分）

### ステップ1: NuGetパッケージを更新

```bash
# DinkToPdfを削除
dotnet remove package DinkToPdf

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2: ネイティブバイナリを削除

プロジェクトからこれらのファイルを削除します：
- `libwkhtmltox.dll` (Windows)
- `libwkhtmltox.so` (Linux)
- `libwkhtmltox.dylib` (macOS)

### ステップ3: Usingステートメントを更新

```csharp
// 以前
using DinkToPdf;
using DinkToPdf.Contracts;

// 以降
using IronPdf;
```

### ステップ4: ライセンスキーを適用

```csharp
// アプリケーションの起動時に追加（Program.csまたはGlobal.asax）
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### ステップ5: 基本的なコード移行

**以前 (DinkToPdf):**
```csharp
using DinkToPdf;
using DinkToPdf.Contracts;

var converter = new SynchronizedConverter(new PdfTools());
var doc = new HtmlToPdfDocument()
{
    GlobalSettings = {
        ColorMode = ColorMode.Color,
        Orientation = Orientation.Portrait,
        PaperSize = PaperKind.A4
    },
    Objects = {
        new ObjectSettings() {
            HtmlContent = "<h1>Hello World</h1>",
            WebSettings = { DefaultEncoding = "utf-8" }
        }
    }
};
byte[] pdf = converter.Convert(doc);
File.WriteAllBytes("output.pdf", pdf);
```

**以降 (IronPDF):**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;

var pdf = renderer.RenderHtmlAsPdf("<h1>Hello World</h1>");
pdf.SaveAs("output.pdf");
// スレッドセーフティの問題、ネイティブバイナリなし！
```

---